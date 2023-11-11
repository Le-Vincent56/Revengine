# Revengine
The Revengine is a custom-built, low-level engine that utilizes C++ for internal/backend programming, and Windows Presentation Framework (WPF) C# for the front-end, Editor programming. The Revengine uses the WPF Model-View-ViewModel (MVVM) Pattern to separate Editor UI scripts from their functionalities.

## Naming Conventions
The Revengine uses some atypical naming conventions for certain fields, properties, and functions/methods.

### Fields & Properties
C# Fields will always start with a "_" (underscore) character to more clearly differentiate them from any property counterparts, which are denoted
by a Capital letter with the same field name. For instance:
`private string _name; // Field
```public string Name;   // Property`

### Functions/Methods
C++ Functions/Methods will use lowerCamelCase, but separate the would-be Uppercase letters with "_" (underscores). This is because the underscore separator, I find, makes the code easier to read, especially for longer functions that might have similar names. It will also help separate them from the C# functions/methods written for WPF, which use the typical UpperCamelCase naming convention. For instance:
`grievance create_grievance(grievance_descriptor* g) { }  // C++
```Grievance CreateGrievance(grievanceDescriptor* g) { }     // C#`
