using System;

namespace NotizApp;

public class Note
{
    public string Titel { get; set; }
    public string Inhalt { get; set; }
    public DateTime Created { get; set; }

    public Note()
    {
        
    }
    
    public Note(string  titel, string inhalt)
    {
        Titel = titel;
        Inhalt = inhalt;
        Created = DateTime.Now;
    }
}