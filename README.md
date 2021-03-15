<h1 align="center">Welcome to Three-layer_architecture_sample 👋</h1>
<p>
</p>

## Author

👤 **Uyen Luu**

* Article: https://enlabsoftware.com/development/how-to-build-and-deploy-a-3-layer-architecture-application-with-c-sharp-net-in-practice.html

## Guideline
**Create database**
* Execute the script at ThreeLayerSample.Infrastructure/Data/ThreeLayerSample.Database.sql

**Add connection between the app to the database**
* Open setting file at ThreeLayerSample.Web(Razor)/appsettings.json
* Replace the connection string by your suiable SQL configuration

**Run the app**
* Open your app in Visual Studio(2019)
* Set ThreeLayerSample.Web(Razor) as the start up project
* Run the app

**Deploy to your IIS**
* Create a folder then publish the ap to that folder.
* Create a site in IIS, point that site to the published folder.
* Launch the site on your browser.

Give a ⭐️ if this project helped you!
