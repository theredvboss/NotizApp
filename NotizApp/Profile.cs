using System.Collections.Generic;

namespace NotizApp;

public class Profile
{
    public string ProfileName { get; set; }
    public List<Note> Notes { get; set; }

    public Profile()
    {
        
    }

    public Profile(string name, List<Note> notes)
    {
        ProfileName = name;
        Notes = notes;
    }
}