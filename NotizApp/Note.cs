namespace NotizApp;

public class Note
{
    public string Titel { get; set; }
    public string Inhalt { get; set; }
    public string createdAt { get; set; }

    public Note()
    {
        
    }
    
    public Note(string  titel, string inhalt, string createdAt)
    {
        Titel = titel;
        Inhalt = inhalt;
        this.createdAt = createdAt;
    }
}