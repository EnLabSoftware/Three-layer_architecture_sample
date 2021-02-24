using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ThreeLayerSample.Infrastructure.Extensions
{
    public static class DbContextExtension
	{
		private static readonly object ConnectionLocked = new object();

		/// <summary>
		/// Creates an initial DbCommand object based on a stored procedure name
		/// </summary>
		/// <param name="context">target database context</param>
		/// <param name="storedProcName">target procedure name</param>
		/// <param name="prependDefaultSchema">Prepend the default schema name to <paramref name="storedProcName"/> if explicitly defined in <paramref name="context"/></param>
		/// <returns></returns>
		public static DbCommand LoadStoredProc(this DbContext context, string storedProcName, bool prependDefaultSchema = true)
		{
			var conn = context.Database.GetDbConnection();
			var cmd = conn.CreateCommand();
			if (prependDefaultSchema)
			{
				var schemaName = context.Model.GetDefaultSchema();
				if (schemaName != null)
				{
					storedProcName = $"{schemaName}.{storedProcName}";
				}

			}

			if (context.Database.CurrentTransaction != null)
			{
				cmd.Transaction = context.Database.CurrentTransaction.GetDbTransaction();
			}

			cmd.CommandText = storedProcName;
			cmd.CommandType = CommandType.StoredProcedure;
			return cmd;
		}

        /// <summary>
        /// Creates a DbParameter object and adds it to a DbCommand
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="direction"></param>
        /// <param name="configureParam"></param>
        /// <returns></returns>
        public static DbCommand WithSqlParam(this DbCommand cmd, string paramName, object paramValue, ParameterDirection direction = ParameterDirection.Input, Action<DbParameter> configureParam = null)
		{
			if (string.IsNullOrEmpty(cmd.CommandText) && cmd.CommandType != CommandType.StoredProcedure)
			{
				throw new InvalidOperationException("Call LoadStoredProc before using this method");
			}

			var param = cmd.CreateParameter();
			param.ParameterName = paramName;
			param.Value = paramValue ?? DBNull.Value;
			param.Direction = direction;
			configureParam?.Invoke(param);
			cmd.Parameters.Add(param);
			return cmd;
		}

        /// <summary>
        /// Creates a DbParameter object and adds it to a DbCommand
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="paramName"></param>
        /// <param name="configureParam"></param>
        /// <returns></returns>
        public static DbCommand WithSqlParam(this DbCommand cmd, string paramName, Action<DbParameter> configureParam = null)
		{
			if (string.IsNullOrEmpty(cmd.CommandText) && cmd.CommandType != CommandType.StoredProcedure)
			{
				throw new InvalidOperationException("Call LoadStoredProc before using this method");
			}

			var param = cmd.CreateParameter();
			param.ParameterName = paramName;
			configureParam?.Invoke(param);
			cmd.Parameters.Add(param);
			return cmd;
		}

		/// <summary>
		/// Creates a DbParameter object based on the SqlParameter and adds it to a DbCommand.
		/// This enabled the ability to provide custom types for SQL-parameters.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="paramName"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static DbCommand WithSqlParam(this DbCommand cmd, string paramName, SqlParameter parameter)
		{
			if (string.IsNullOrEmpty(cmd.CommandText) && cmd.CommandType != CommandType.StoredProcedure)
			{
				throw new InvalidOperationException("Call LoadStoredProc before using this method");
			}

			//var param = cmd.CreateParameter();
			//param.ParameterName = paramName;
			//configureParam?.Invoke(param);
			cmd.Parameters.Add(parameter);

			return cmd;
		}

		public class SprocResults
		{

			//  private DbCommand _command;
			private DbDataReader _reader;

			public SprocResults(DbDataReader reader)
			{
				// _command = command;
				_reader = reader;
			}

			public List<T> ReadToList<T>()
			{
				return MapToList<T>(_reader);
			}

			public List<T> ReadNextListOrEmpty<T>()
			{
				var items = ReadToList<T>();

				_reader.NextResult();

				return items ?? new List<T>();
			}

			public T? ReadToValue<T>() where T : struct
			{
				return MapToValue<T>(_reader);
			}

			public Task<bool> NextResultAsync()
			{
				return _reader.NextResultAsync();
			}

			public Task<bool> NextResultAsync(CancellationToken ct)
			{
				return _reader.NextResultAsync(ct);
			}

			public bool NextResult()
			{
				return _reader.NextResult();
			}

			/// <summary>
			/// Retrieves the column values from the stored procedure and maps them to <typeparamref name="T"/>'s properties
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="dr"></param>
			/// <returns>List<<typeparamref name="T"/>></returns>
			private List<T> MapToList<T>(DbDataReader dr)
			{
				var objList = new List<T>();
				var props = typeof(T).GetRuntimeProperties().ToList();

				var colMapping = dr.GetColumnSchema()
					.Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
					.ToDictionary(key => key.ColumnName.ToLower());

				if (dr.HasRows)
				{
					while (dr.Read())
					{
						T obj = Activator.CreateInstance<T>();
						foreach (var prop in props)
						{
							if (colMapping.ContainsKey(prop.Name.ToLower()))
							{
								var column = colMapping[prop.Name.ToLower()];

								if (column?.ColumnOrdinal != null)
								{
									var val = dr.GetValue(column.ColumnOrdinal.Value);
									if (prop.CanWrite)
									{
										prop.SetValue(obj, val == DBNull.Value ? null : val);
									}
								}

							}
						}
						objList.Add(obj);
					}
				}
				return objList;
			}

			/// <summary>
			///Attempts to read the first value of the first row of the result set.
			/// </summary>
			private T? MapToValue<T>(DbDataReader dr) where T : struct
			{
				if (dr.HasRows)
				{
					if (dr.Read())
					{
						return dr.IsDBNull(0) ? new T?() : dr.GetFieldValue<T>(0);
					}
				}
				return new T?();
			}
		}

        /// <summary>
        /// Executes a DbDataReader and returns a list of mapped column values to the properties of <typeparamref>
        ///     <name>T</name>
        /// </typeparamref>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="handleResults"></param>
        /// <param name="commandBehaviour"></param>
        /// <param name="manageConnection"></param>
        /// <returns></returns>
        public static void ExecuteStoredProc(this DbCommand command, Action<SprocResults> handleResults, 
            CommandBehavior commandBehaviour = CommandBehavior.Default, bool manageConnection = false)
		{
			if (handleResults == null)
			{
				throw new ArgumentNullException(nameof(handleResults));
			}

			using (command)
			{
				lock (ConnectionLocked)
				{
					if (command.Connection.State == ConnectionState.Closed)
					{
						command.Connection.Open();
					}
				}
				try
                {
                    using var reader = command.ExecuteReader(commandBehaviour);
                    var sprocResults = new SprocResults(reader);
                    handleResults(sprocResults);
                }
				finally
				{
					if (manageConnection)
					{
						command.Connection.Close();
					}
				}
			}
		}

        /// <summary>
        /// Executes a DbDataReader asynchronously and returns a list of mapped column values to the properties of <typeparamref>
        ///     <name>T</name>
        /// </typeparamref>
        /// .
        /// </summary>
        public static async Task ExecuteStoredProcAsync(this DbCommand command, Action<SprocResults> handleResults, CommandBehavior commandBehaviour = CommandBehavior.Default, CancellationToken ct = default, bool manageConnection = false)
		{
			if (handleResults == null)
			{
				throw new ArgumentNullException(nameof(handleResults));
			}

			using (command)
			{
				lock (ConnectionLocked)
				{
					if (command.Connection.State == ConnectionState.Closed)
					{
						command.Connection.Open();
					}
				}
				try
				{
					using (var reader = await command.ExecuteReaderAsync(commandBehaviour, ct).ConfigureAwait(false))
					{
						var results = new SprocResults(reader);
						handleResults(results);
					}
				}
				finally
				{
					if (manageConnection)
					{
						command.Connection.Close();
					}
					command.Dispose();
				}
			}
		}

		public static async Task<List<T>> ExecuteForListAsync<T>(this DbCommand command, CommandBehavior commandBehaviour = CommandBehavior.Default, CancellationToken ct = default, bool manageConnection = false)
			where T : class
		{
			List<T> result = null;

			await command.ExecuteStoredProcAsync((d) => { result = d.ReadNextListOrEmpty<T>(); }, commandBehaviour, ct, manageConnection);

			return result;
		}

		public static async Task<T> ExecuteForFirstAsync<T>(this DbCommand command, CommandBehavior commandBehaviour = CommandBehavior.Default, CancellationToken ct = default, bool manageConnection = false)
			where T : class
		{
			T result = null;

			await command.ExecuteStoredProcAsync((d) => { result = d.ReadNextListOrEmpty<T>().FirstOrDefault(); }, commandBehaviour, ct, manageConnection);

			return result;
		}
	}
}
