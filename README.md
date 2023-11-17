# Revengine
The Revengine is a custom-built, low-level engine that utilizes C++ for internal/backend programming, and Windows Presentation Framework (WPF) C# for the front-end, Editor programming. The Revengine uses the WPF Model-View-ViewModel (MVVM) Pattern to separate Editor UI scripts from their functionalities.

## Naming Conventions
The Revengine uses some atypical naming conventions for certain fields, properties, and functions/methods.

### Fields & Properties
C# Fields will always start with a "_" (underscore) character to more clearly differentiate them from any property counterparts, which are denoted
by a Capital letter with the same field name. For instance:
```
private string _name; // Field
public string Name;   // Property
```

### Functions/Methods
C++ Functions/Methods will use lowerCamelCase, but separate the would-be Uppercase letters with "_" (underscores). This is because the underscore separator, I find, makes the code easier to read, especially for longer functions that might have similar names. It will also help separate them from the C# functions/methods written for WPF, which use the typical UpperCamelCase naming convention. For instance:
```
grievance create_grievance(grievance_descriptor* g) { }  // C++
Grievance CreateGrievance(grievanceDescriptor* g) { }    // C#
```

## Terminology
The Revengine uses a basic Project-Scene-GameObject-Component hierarchy, but uses some different terminology to fit with the name. Here's some equivalencies from Unity to the Revengine:
* GameObject -> Grievance
* Component -> Motivator

## Goals/Checklist
* Project Template Creating/Loading                             ✅
* Project Scene Add/Remove                                      ✅
* Grievances and Motivators - Editor                            ✅
* Editor Undo/Redo System                                       ✅
* Editor Message Log                                            ✅
* Editor Grievance/Motivator Multiselection                     ✅
* Generation-Index Identifiers                                  ✅
* Grievances and Motivators - Engine                            ✅
* Engine-to-Editor Pipeline DLL                                 ✅
* Custom Script Motivators                          
* Visual Studio Solution/Project Auto-Generation (EnvDTE)       ✅
* Visual Studio Source File Generation                          ✅
* DLL for Loading Game Code
* Project Saving in Binary Format
* Editor - Custom Game Window
* Geometry Pipeline
* View and Projection Matrices
* Rotations and Quaternions
* World Planes for Scenes
* Mesh Renderers
* Camera Movement
* Input Handling
* Lights (Point/Spotlight)
