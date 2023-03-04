```
Author:     Landon West
Partner:    None
Date:       24-Feb-2023
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  landonwest815
Repo:       https://github.com/uofu-cs3500-spring23/spreadsheet-landonwest815
Date:       03-Mar-2023 7:15pm (when submission was completed) 
Project:    GUI
Copyright:  CS 3500 and Landon West - This work may not be copied for use in Academic Coursework.
```
 
# Comments to Evaluators:

    PLEASE READ PRIOR TO THE START OF THE GRADING PROCESS:
        
        My submitted code initializes the spreadsheet with columnns 'A'-'Z' and rows 1-99.
        This was chosen becuase of what the assignment specifications mentioned.
        HOWEVER, I would reccomend that you change the 'initialTopColumns' and 'numOfLeftLabels' to lower values.
        Not only does this allow the program to open faster on your part, but it also allows you to experience my
        Adding Columns feature. Thank you.

    Note on the Color Changing Methods:

        I am pretty sure that these methods could be simplified down using knowledge of EventArgs. However, I did not want to spend
        a lot of time learning how to use those and since this feature wasn't a part of the assignment, I simply created a method for
        each color. I would appreciate if this did not lose me points for the reasons mentioned.

    Note on the Adding Columns Feature:

        As of now, the user can only add additional columns up to the label: 'Z'. If time had permitted, I wanted to implement
        an algorithm that would find the next label. For example, 'AZ' would increment to 'BA'. I decided this would take
        a decent amount of time and brain power, and since this was not a part of the assignment I decided to simply limit
        the labels to 'A' through 'Z'. I would also appreciate if this did not lose me points for the reasons mentioned.

    Note on Opening Spreadsheets That Don't Fit on the Current Spreadsheet Editor:
        
        In this Spreadsheet Editor you can add additional rows and columns as the user desires. However, this creates a
        small problem. Take the following scenario as an example: you start with 10 rows, add 5 more, and save the 
        spreadsheet with information in the 15th row. If you then exit the program and reopen it, the editor will revert
        back to the original 10 rows. Now, if you try to open the spreadsheet you created, the program will tell the you
        that it cannot be opened. This is because spreadsheets don't save size information. I was planning on adding such
        details to my EnhancedSpreadsheet class, however, this required modifying the XML reading & writing functionality
        and caused a lot of confusion real fast. I didn't want to spend mass amounts of time trying to figure this out, and
        instead focused on the assignment specifications. Although, this can cause problems when opening a spreadsheet that
        is bigger than the current state of the editor, the program still takes care of this and prevents crashing. I think
        that this was a good compromise for the time required and the rarity of causing this problem to arise. Additionally,
        this problem only happens when the columns are set to a size of less than 26. (Columns don't reach 'Z') Since my
        program is set to have all columns by default, the only way to cause the problem described above is by changing
        the actual code. Thus, I would appreciate if this did not dock me any points.

# Assignment Specific Topics

    My Additonal Features:

        1. Adding Additional Rows/Columns After Spreadsheet Creation
            
            After creation, the user is allowed to add additional rows & columns to their spreadsheet. This is done by simply
            pressing the '+' buttons next to the end of the row and column labels.

            If the user tries to open a spreadsheet that contains more rows/columns than they currently have initialized, the
            program will let the user know that they need more rows/columns. Such spreadsheet cannot be opened until there is
            a suffice number of rows/columns.

            The number of rows can be incremented as much as the user desires. However, their machine might not appreciate it
            and slow down if enough rows are added.

            The number of columns can only be incremented up to the letter 'Z'. The reasoning for this is stated above in
            the 'Comments to Evaluators' section.

        2. Changing the Theme of the Spreadsheet

            Located to the right of the File Menu, the user can access a drop down menu that contains numerous colors.
            Selecting a color will change the theme of the Spreadsheet in an instant. The default them is set to 'Orange'.

        3. Changing Cell Contents via .Unfocus()
            
            NOTICE: Although this isn't necessarily an 'additional feature', I wanted to include it

            Rather than having to press enter to submit contents into a cell, all the user has to do is type it. Unfocusing
            a cell via pressing enter, tab, or clicking else where will automatically save the contents into the spreadsheet.
            This was not a specification for the assignment, however, I felt it would greatly improve the feel of the program.

# Examples of Good Software Practice

    DRY:
        I made use of the DRY process a lot in this assignment. I tried to structure my helper methods in a way that
        helps the process make sense. I like to break everything into their separate functions and then connect them
        all together by calling each methods as needed. This was the case a lot when checking for data loss. The data
        loss method would be called, and then passed into the necesary method afterwards.

    Well named, commented methods:
        I believe I made use of well named and commented methods very nicely in this assignment. Every method has a header 
        comment with details on what the method does and method specific topics. The main flow of each method is well 
        commented if it needs extra definition. All variable are named in a way that makes the code more readable.

    Abstraction:
        This final assignment makes extremely good use of abstraction. Numerous structures inherit from parent classes
        and implement additional features on top.

# Consulted Peers:
 
    I did not talk with anyone for this assignment. 
    All my questions were answered through the links mentioned below.
    Piazza also contained a lot of tips that helped.
    Additionally, a lot of my questions were also answered by simply attending the lectures.

# References:

    NOTICE: Not all of these references actually led to results in my code
            Some did and some did not. However, I figured I might aswell include
            all websites that I visited throughout the process.

    1. Exploring Layout Options in .NET MAUI - https://www.telerik.com/blogs/exploring-layout-options-dotnet-maui
    2. How to avoid a System.Runtime.InteropServices.COMException? - https://stackoverflow.com/questions/4281425/how-to-avoid-a-system-runtime-interopservices-comexception
    3. Remove underline from Entry Control in Xamarin Forms - https://stackoverflow.com/questions/58099796/remove-underline-from-entry-control-in-xamarin-forms
    4. Apple Calculator App Icon 2017 Color Scheme - https://www.schemecolor.com/apple-calculator-app-icon-2017-colors.php
    5. .net maui windows titlebar - https://stackoverflow.com/questions/71354817/net-maui-windows-titlebar
    6. Display pop-ups - https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pop-ups?view=net-maui-7.0
    7. Object.GetType Method - https://learn.microsoft.com/en-us/dotnet/api/system.object.gettype?view=net-7.0
    8. Double.TryParse Method - https://learn.microsoft.com/en-us/dotnet/api/system.double.tryparse?view=net-7.0
    9. How to Exit a Method without Exiting the Program? - https://stackoverflow.com/questions/3314305/how-to-exit-a-method-without-exiting-the-program
    10. .NET MAUI windows - https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/windows?view=net-maui-7.0
    11. How to gracefully shutdown (or put to sleep) a .Net Maui App - https://stackoverflow.com/questions/71022524/how-to-gracefully-shutdown-or-put-to-sleep-a-net-maui-app
    12. Error CS0051 (Inconsistent accessibility: parameter type 'Job' is less accessible than method 'AddJobs.TotalPay(Job)') - https://stackoverflow.com/questions/4060703/error-cs0051-inconsistent-accessibility-parameter-type-job-is-less-accessibl
    13. Best way to read, modify, and write XML - https://stackoverflow.com/questions/3736516/best-way-to-read-modify-and-write-xml
    14. Input string was not in a correct format - https://stackoverflow.com/questions/8321514/input-string-was-not-in-a-correct-format
    16. Iterating through the Alphabet - C# a-caz - https://stackoverflow.com/questions/1011732/iterating-through-the-alphabet-c-sharp-a-caz
    17. Catch multiple exceptions at once? - https://stackoverflow.com/questions/136035/catch-multiple-exceptions-at-once
    18. Border - https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/border?view=net-maui-7.0
    19. Colors - https://learn.microsoft.com/en-us/dotnet/maui/user-interface/graphics/colors?view=net-maui-7.0
    20. Fonts in .NET MAUI - https://learn.microsoft.com/en-us/dotnet/maui/user-interface/fonts?view=net-maui-7.0
    21. VerticalStackLayout - https://learn.microsoft.com/en-us/dotnet/maui/user-interface/layouts/verticalstacklayout?view=net-maui-7.0
    22. StackLayout - https://learn.microsoft.com/en-us/dotnet/maui/user-interface/layouts/stacklayout?view=net-maui-7.0
    23. What is a NullReferenceException, and how do I fix it? - https://stackoverflow.com/questions/4660142/what-is-a-nullreferenceexception-and-how-do-i-fix-it
    24. .NET MAUI Entry Events - https://docs.telerik.com/devtools/maui/controls/entry/events
    25. VisualElement.Focused Event - https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.visualelement.focused?view=net-maui-7.0
    26. Window.Title Property - https://learn.microsoft.com/en-us/dotnet/api/system.windows.window.title?view=windowsdesktop-7.0
    27. Button - https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/button?view=net-maui-7.0
    28. Entry - https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/entry?view=net-maui-7.0
    29. How to get a path to the desktop for current user in C#? - https://stackoverflow.com/questions/634142/how-to-get-a-path-to-the-desktop-for-current-user-in-c/634145
    30. Char.IsLetter Method - https://learn.microsoft.com/en-us/dotnet/api/system.char.isletter?view=net-7.0