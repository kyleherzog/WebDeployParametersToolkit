# Roadmap
- [ ] Add explicit menu option to update Parameters.xml
- [ ] SetParameters.xml intellisense

Features that have a checkmark are complete and available for
download in the
[CI build](http://vsixgallery.com/extension/6435437e-72fb-4626-9a47-865f185ce258/).

# Changelog

These are the changes to each version that has been released
on the official Visual Studio extension gallery.

## 3.2
**2020-7-18**
- [x] Fix for Visual Studio 2017 support

## 3.1
**2020-5-2**
- [x] Added asynchronous loading support

## 3.0
**2018-12-9**
- [x] Visual Studio 2019 support
- [x] Web.config "location" element support

## 2.0
**2017-3-5**
- [x] Option to set default parameters to values from web.config or to tokenized values

## 1.8
**2017-3-4**
- [x] Generate parameters for appSettings
- [x] Open files in editor window after being generated/updated

## 1.7
**2017-2-18**
- [x] Skip generating parameters for StringCollections and any other application settings not serialized as strings

## 1.6
**2017-1-10**
- [x] Visual Studio 2017 support

## 1.5
**2016-11-5**
- [x] User configurable options for which parameters to generate

## 1.4
**2016-10-15**
- [x] Generate parameter for compilation debug attribute
- [x] Generate parameters for mailSettings
- [x] Generate parameters for session state settings
- [x] Bug fix for *Import Missing Parameters*

## 1.3
**2016-9-26**

- [x] Configure project to copy SetParameters.xml files to drop location during build


## 1.2
**2016-09-07**

- [x] Handling missing connectionStrings node in web.config file

## 1.1

**2016-09-02**

- [x] Fix for *setParameters* element being wrongly named *parameters* instead when generating SetParameters.xml files

## 1.0

**2016-08-31**

- [x] Initial release
- [x] Generate Parameters.xml from web.config
- [x] Generate SetParameters.xml files from web.config
- [x] Import missing parameters into SetParameters.xml file
- [x] Nest SetParameters.xml files under Parameters.xml file 







