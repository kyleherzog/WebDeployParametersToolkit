# Web Deploy Parameters Toolkit

[![Build status](https://ci.appveyor.com/api/projects/status/pjlmikxue6721vab?svg=true)](https://ci.appveyor.com/project/kyleherzog/webdeployparameterstoolkit)

Download this extension from the [VS Marketplace](https://marketplace.visualstudio.com/items?itemName=kherzog.WebDeployParametersToolkit)
or get the [CI build](http://vsixgallery.com/extension/6435437e-72fb-4626-9a47-865f185ce258/).

---------------------------------------

Tools to make it easy to work with parameters in Web Deploy (MSDeploy).

See the [changelog](CHANGELOG.md) for changes and roadmap.

## Features

- Generate Parameters.xml from Web.Config
- Generate SetParameters[Environment].xml
- Import missing parameters into SetParameters.xml files
- Nest SetParameters.xml files under Parameters.xml


### Generate Parameters.xml from Web.Config
Select the web.config in solution explorer and right-click.

![Web Config Context Menu](art/WebConfigContextMenu.png)

Then select *Generate Parameters.xml* to have a Parameters.xml file automatically generated from settings in the web.config file. If a Parameters.xml file already exists in the project, missing parameters will be merged into the existing Parameters.xml file.

To control which settings are automatically generated, adjust the settings in the Visual Studio Tools > Options dialog.
![Options Dialog](art/OptionsDialog.png)

Starting in version 2.0, the options dialog also allows the user to select how the default values are generated in the Parametrs.xml file.  The default style is clone, which will read the value from the web.config file and copy it into the default value attribute.  The other option is tokenize.  Tokenize will generate the default value in the format of \_\_PROPERTYNAME__.

### Generate SetParameters[Environment].xml
Select the Parameters.xml file in solution explorer and right-click.

![Parameters Xml Context Menu](art/ParametersXmlContextMenu.png)

Then select *Generate SetParameters.xml*. 

A dialog will then give you the option to add an environment name to the new SetParameters.xml file to be generated.

![Set Parameters Name Dialog](art/SetParametersNameDialog.png)

Either enter an envionment name or leave it blank and then click *Create*. Then the new SetPameters.xml file is created.

### Import Missing Parameters into SetParameters.xml files
If parameters have been added to the Parameters.xml file, the new parameters can be automatically added to a SetParameters.xml file.  Just select the SetParameters.xml file in solution explorer and right-click.

![Set Parameters Import Menu](art/SetParametersImportMenu.png)

Then select *Import Missing Parameters* to have any missing parameters automatically added to the selected file.

### Nest SetParameters.xml files under Parameters.xml
SetParameters*.xml files will automatically be nested under the Parameters.xml file. 

To nest a SetParameters*.xml file that was created prior to installing this extension. Select the SetParameters*.xml file in solution explorer and right-click.

![Set Parameters Nest Menu](art/SetParametersNestMenu.png)

Then select *Nest Under Parameters.xml* to have the selected file nested under Parameters.xml.

### Parameterization Build Action Support
When generating or nesting SetParameters*.xml files, the build action on those files will be set to *Parameterization*.

This setting will result in these files being copied to the package location and excluded from the package itself.


## Contribute
For cloning and building this project yourself, make sure
to install the
[Extensibility Tools](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.ExtensibilityTools)
extension for Visual Studio which enables some features
used by this project.

## License
[Apache 2.0](LICENSE)