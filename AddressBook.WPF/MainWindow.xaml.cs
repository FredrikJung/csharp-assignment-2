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
        private ObservableCollection<Contact> _contacts;
        private readonly IFileManager _fileManager;
        private readonly string _filePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\AddressBookWPF.json";

        public MainWindow(IFileManager fileManager)
        {
            InitializeComponent();
            _contacts = new ObservableCollection<Contact>();
            _fileManager = fileManager;
            UpdateContactList();
            btn_Save.Visibility = Visibility.Hidden;
            btn_GoBack.Visibility = Visibility.Hidden;
        }
        private void UpdateContactList()
        {
            try
            {
                var list = _contacts = JsonConvert.DeserializeObject<ObservableCollection<Contact>>(_fileManager.Read(_filePath))!;
                lv_Contacts.ItemsSource = _contacts?.OrderBy(x => x.FirstName);
            }
            catch { }
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            var contact = _contacts.FirstOrDefault(x => x.PhoneNumber == tb_PhoneNumber.Text);

            if(contact == null)
            {
                if(tb_FirstName.Text == "" || tb_PhoneNumber.Text == "")
                {
                    MessageBox.Show("Förnamn och telefonnummer är obligatoriska att fylla i");
                }
                else
                {
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
            else
            {
                MessageBox.Show("Det finns redan en kontakt med samma telefonnummer");
            }
            ClearTextBox();
        }
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

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var contact = (Contact)button!.DataContext;
            _contacts.Remove(contact);
            _fileManager.Save(_filePath, JsonConvert.SerializeObject(_contacts, Formatting.Indented));
            UpdateContactList();
            ClearTextBox();

            btn_Add.Visibility = Visibility.Visible;
            btn_Save.Visibility = Visibility.Hidden;
            btn_GoBack.Visibility = Visibility.Hidden;
        }

        private void lv_Contacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var contact = lv_Contacts.SelectedItem as Contact;

            if(contact != null)
            {
                tb_FirstName.Text = contact.FirstName;
                tb_LastName.Text = contact.LastName; ;
                tb_PhoneNumber.Text = contact.PhoneNumber; ;
                tb_Email.Text = contact.Email.ToLower(); ;
                tb_StreetName.Text = contact.StreetName; ;
                tb_PostalCode.Text = contact.PostalCode; ;
                tb_City.Text = contact.City; ;

                btn_Add.Visibility = Visibility.Hidden;
                btn_Save.Visibility = Visibility.Visible;
                btn_GoBack.Visibility = Visibility.Visible;
            }
 
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            var contact = lv_Contacts.SelectedItem as Contact;

            contact!.FirstName = tb_FirstName.Text;
            contact!.LastName = tb_LastName.Text;
            contact!.PhoneNumber = tb_PhoneNumber.Text;
            contact!.Email = tb_Email.Text;
            contact!.StreetName = tb_StreetName.Text;
            contact!.PostalCode = tb_PostalCode.Text;
            contact!.City = tb_City.Text;

            _fileManager.Save(_filePath, JsonConvert.SerializeObject(_contacts, Formatting.Indented));
            UpdateContactList();
            ClearTextBox();

            btn_Add.Visibility = Visibility.Visible;
            btn_Save.Visibility = Visibility.Hidden;
        }

        private void btn_GoBack_Click(object sender, RoutedEventArgs e)
        {
            UpdateContactList();
            ClearTextBox();

            btn_Add.Visibility = Visibility.Visible;
            btn_Save.Visibility = Visibility.Hidden;
            btn_GoBack.Visibility = Visibility.Hidden;
        }
    }
}
