# Storm.BuildTasks [![Build status](https://ci.appveyor.com/api/projects/status/av1ykcbj99ll4a0e/branch/develop?svg=true)](https://ci.appveyor.com/project/Julien-Mialon/storm-buildtasks/branch/develop)

This repository contains MSBuild tasks to simplify development of (mostly) Xamarin applications.

Here is the list of tasks available : 
* [Storm.BuildTasks.AndroidColors](#stormbuildtasksandroidcolors-) : share colors between apps in C# file as constant and use this task to turn C# file into colors.xml for Android


## Storm.BuildTasks.AndroidColors [![nuget](https://img.shields.io/nuget/v/Storm.BuildTasks.AndroidColors.svg)](https://www.nuget.org/packages/Storm.BuildTasks.AndroidColors)

Available on [nuget](https://www.nuget.org/packages/Storm.BuildTasks.AndroidColors) or with nuget Package Manager `Install-Package Storm.CrossLocalization`

This nuget package add a build tasks before compilation of your project to turn C# file with int variable into colors.xml file format for Xamarin.Android. 

Main purpose: Sharing colors between Xamarin.Android and Xamarin.iOS projects easily.

### How to ?
* Reference nuget package
* Add your C# file containing your colors in the directory where you want your color.xml to be generated (for instance: Resources/values). If it's shared with other projects, you can add it as link !
* Set its build target to `ColorFile`
* Build your project
* A colors.xml file has been generated next to your C# file, you just need to add it to the project.
* Bonus: if you have multiple C# file in the same directory, they will be turned into only one colors.xml file containing all values.


# Contributions
Contributions are welcome !
* If you find a bug, please fill an issue
* If you want a feature for an existing task, please fill an issue
* If you want another task that you think can be useful, please fill an issue, we can discuss about it

If you want to contribute, create a pull request or fill an issue to discuss about it before !

# Licence
MIT © [Julien Mialon](https://github.com/Julien-Mialon)