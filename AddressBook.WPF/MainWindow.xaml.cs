using AddressBook.WPF.Models;
using AddressBook.WPF.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AddressBook.WPF
{
    public partial class MainWindow : Window
    {
        //Säger att det ska finnas en ObservableCollection lista av typen Contact
        private ObservableCollection<Contact> _contacts;

        //Sätter sökvägen till filen
        private readonly string _filePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\AddressBookWPF.json";

        //Lägger till min Filemanager med dependency injection
        private readonly IFileManager _fileManager;

        public MainWindow(IFileManager fileManager)
        {
            InitializeComponent();

            _fileManager = fileManager;

            //Skapar en listan när programmet startas upp
            _contacts = new ObservableCollection<Contact>();

            //Anväder metoden för att rensa alla fält när programmet startas upp
            UpdateContactList();

            //Döljer min "Spara" knapp när programmet startas upp
            btn_Save.Visibility = Visibility.Hidden;

            //Döljer min "Tillbaka" knapp när programmet startas upp
            btn_GoBack.Visibility = Visibility.Hidden;
        }

        //Metod för titta om det finns en lista på angiven sökväg
        //Läser i så fall in den och konverterar om det från json format till en ObservableCollection
        //Visar upp listan i appen och sorterar den i bokstavsordrning på förnamn
        private void UpdateContactList()
        {
            try
            {
                var list = _contacts = JsonConvert.DeserializeObject<ObservableCollection<Contact>>(_fileManager.Read(_filePath))!;
                lv_Contacts.ItemsSource = _contacts?.OrderBy(x => x.FirstName);
            }
            catch { }
        }

        //Metod för vad som ska hända när man klickar på "Lägg till" knappen
        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            //Tittar i listan ifall det finns ett objekt med identiskt telefonnummer som man skrivit in i fältet för telefonnummer
            var contact = _contacts.FirstOrDefault(x => x.PhoneNumber == tb_PhoneNumber.Text);

            //IF-sats som kollar om angivet telfonummer inte finns
            if(contact == null)
            {
                //IF-sats sedan kollar att förnamn och telefonnummer är ifyllt annars visas nedan felmeddelande
                if(tb_FirstName.Text == "" || tb_PhoneNumber.Text == "")
                {
                    MessageBox.Show("Förnamn och telefonnummer är obligatoriska att fylla i");
                }
                else
                {
                    //Om både förnamn och telefonnummer är i fyllt och telefonnumret inte finns i annan kontakt så läggs kontakten till och sparas i listan
                    //Listan sparas till sökvägen i json format samtidigt som den json listan formateras till en mer läsvänligform
                    _contacts.Add(new Contact
                    {

                        FirstName = tb_FirstName.Text,
                        LastName = tb_LastName.Text,
                        PhoneNumber = tb_PhoneNumber.Text,
                        Email = tb_Email.Text,
                        StreetName = tb_StreetName.Text,
                        PostalCode = tb_PostalCode.Text,
                        City = tb_City.Text

                    });
                    _fileManager.Save(_filePath, JsonConvert.SerializeObject(_contacts, Formatting.Indented));
                    UpdateContactList();
                }
            }

            //Visar nedan felmedelande ifall angivet telefonnummer redan skulle finnas i en befintlig kontakt i listan
            else
            {
                MessageBox.Show("Det finns redan en kontakt med samma telefonnummer");
            }
            ClearTextBox();
        }

        //Metod för att rensa alla värden man fyllt i formuläret
        private void ClearTextBox()
        {
            tb_FirstName.Text = "";
            tb_LastName.Text = "";
            tb_PhoneNumber.Text = "";
            tb_Email.Text = "";
            tb_StreetName.Text = "";
            tb_PostalCode.Text = "";
            tb_City.Text = "";
        }

        //Metod för man vad som ska hända ifall men klickar på "Kryss" ikonen
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            //Talar om att det här är en knapp
            var button = sender as Button;

            //Hämtar informationen från objektet där knappen generades och sparar in i en variabel av typen Contact
            var contact = (Contact)button!.DataContext;

            //Tar bort kontakten som vi hämtat från listan
            _contacts.Remove(contact);

            //Sparar om listan
            _fileManager.Save(_filePath, JsonConvert.SerializeObject(_contacts, Formatting.Indented));

            //Läser om listan
            UpdateContactList();

            //Rensar fälten
            ClearTextBox();

            //Gör "Lägg till" knappen synlig samt "Spara" och "Tillbaka" knappen osynliga
            btn_Add.Visibility = Visibility.Visible;
            btn_Save.Visibility = Visibility.Hidden;
            btn_GoBack.Visibility = Visibility.Hidden;
        }

        //Metod för att uppdatera en kontakt
        private void lv_Contacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Sparar in objektet som är valt av typen Contact till en variabel 
            var contact = lv_Contacts.SelectedItem as Contact;

            //Skriver in ovan valt objekts information i reskektive fält i formuläret
            if(contact != null)
            {
                tb_FirstName.Text = contact.FirstName;
                tb_LastName.Text = contact.LastName; ;
                tb_PhoneNumber.Text = contact.PhoneNumber; ;
                tb_Email.Text = contact.Email.ToLower(); ;
                tb_StreetName.Text = contact.StreetName; ;
                tb_PostalCode.Text = contact.PostalCode; ;
                tb_City.Text = contact.City; ;

                //Gör "Lägg till" knappen osynlig samt "Spara" och "Tillbaka" knappen synliga
                btn_Add.Visibility = Visibility.Hidden;
                btn_Save.Visibility = Visibility.Visible;
                btn_GoBack.Visibility = Visibility.Visible;
            }
 
        }

        //Metod för att spara en om en befintlig kontakt
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            //Sparar in objektet som är valt av typen Contact till en variabel 
            var contact = lv_Contacts.SelectedItem as Contact;

            //IF-sats för om man tar bort värdet helt från fältet Förnamn och klickar på "Spara" knappen så visas nedan felmeddelande
            if (tb_FirstName.Text == "")
            {
                MessageBox.Show("Förnamn är obligatoriskt att fylla i");

            }
            else
            {
                //Fyllar man i ett nytt värde UTAN ann lämna det tom och klickar på spara så uppdateras då förnamnet till nya värdet
                contact!.FirstName = tb_FirstName.Text;
            }

            //Nya värdet i fältet Efternamn uppdateras i befintlig kontakt (kan vara tomt)
            contact!.LastName = tb_LastName.Text;

            //IF-sats för om man tar bort värdet helt från fältet Telefonnummer och klickar på "Spara" knappen så visas nedan felmeddelande
            if (tb_PhoneNumber.Text == "")
            {
                MessageBox.Show("Telefonnummer är obligatoriskt att fylla i");
            }

            else
            {
                //Fyllar man i ett nytt värde UTAN ann lämna det tom och klickar på spara så uppdateras då förnamnet till nya värdet
                contact!.PhoneNumber = tb_PhoneNumber.Text;
            }

            //Nya värdet i fältet E-postadress uppdateras i befintlig kontakt (kan vara tomt)
            contact!.Email = tb_Email.Text;

            //Nya värdet i fältet Gatuadress uppdateras i befintlig kontakt (kan vara tomt)
            contact!.StreetName = tb_StreetName.Text;

            //Nya värdet i fältet Postnummer uppdateras i befintlig kontakt (kan vara tomt)
            contact!.PostalCode = tb_PostalCode.Text;

            //Nya värdet i fältet Ort uppdateras i befintlig kontakt (kan vara tomt)
            contact!.City = tb_City.Text;

            //Sparar om listan
            _fileManager.Save(_filePath, JsonConvert.SerializeObject(_contacts, Formatting.Indented));

            //Läser om listan
            UpdateContactList();

            //Rensar fälten från värden
            ClearTextBox();

            //Gör "Lägg till" knappen synlig samt "Spara" och "Tillbaka" knappen osynliga
            btn_Add.Visibility = Visibility.Visible;
            btn_Save.Visibility = Visibility.Hidden; 
            btn_GoBack.Visibility = Visibility.Hidden;
        }

        //Metod för vad som ska hända vid klicka på knappen "Tillbaka"
        private void btn_GoBack_Click(object sender, RoutedEventArgs e)
        {
            //Läser om listan utan att spara den
            UpdateContactList();

            //Rensar fälten från värden
            ClearTextBox();

            //Gör "Lägg till" knappen synlig samt "Spara" och "Tillbaka" knappen osynliga
            btn_Add.Visibility = Visibility.Visible;
            btn_Save.Visibility = Visibility.Hidden;
            btn_GoBack.Visibility = Visibility.Hidden;
        }
    }
}
