using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace ThreeLayerSample.Domain.CustomGenerator
{
    class MyEntityTypeGenerator : CSharpEntityTypeGenerator
    {
        public MyEntityTypeGenerator([NotNull] IAnnotationCodeGenerator annotationCodeGenerator, [NotNull] ICSharpHelper cSharpHelper) : base(annotationCodeGenerator, cSharpHelper)
        {
        }

        /// <summary>
        /// Override function to make all entities inherit from EntityBase<int>
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="namespace"></param>
        /// <param name="useDataAnnotations"></param>
        /// <returns></returns>
        public override string WriteCode(IEntityType entityType, string @namespace, bool useDataAnnotations)
        {
            string code = base.WriteCode(entityType, @namespace, useDataAnnotations);

            var oldString = "public partial class " + entityType.Name;
            var newString = "public partial class " + entityType.Name + " : EntityBase<int>";

            var oldId = "public int Id { get; set; }";

            return code.Replace(oldString, newString).Replace(oldId, string.Empty);
        }
    }
}
