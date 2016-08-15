using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SQLite;
using System.IO;
using System.Collections.Generic;

namespace BookList
{
    [Activity(Label = "BookList", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Button btnAdd;
        EditText txtTitle;
        EditText txtISBN;
        ListView tblBooks;
        string filePath
                = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "BookList.db3");

        private void PopulateListView()
        {

            var db = new SQLiteConnection(filePath);

            var bookList = db.Table<Book>();

            List<string> bookTitles = new List<string>();

            foreach (var book in bookList) { bookTitles.Add(book.BookTitle); }

            tblBooks.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, bookTitles.ToArray());



        }
        protected override void OnCreate(Bundle bundle)
        {
            
            base.OnCreate(bundle);
            
            try
            {
                // Create our connection, if the database and/or table doesn't exist create it
                var db = new SQLiteConnection(filePath);
                db.CreateTable<Book>();
                
            }
            catch (IOException ex)
            {
                var reason = string.Format("Failed to create Table - reason {0}", ex.Message);
            Toast.MakeText(this, reason, ToastLength.Long).Show();
            }
            SetContentView(Resource.Layout.Main);

            
            txtTitle = FindViewById<EditText>(Resource.Id.txtTitle);
            txtISBN = FindViewById<EditText>(Resource.Id.txtISBN);
            tblBooks = FindViewById<ListView>(Resource.Id.tblBooks);
            btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            btnAdd.Click += btnAdd_Click;
            PopulateListView();


        }
        
        void btnAdd_Click(object sender, System.EventArgs e)
        {
            
            string alertTitle, alertMessage;
            
            if (!string.IsNullOrEmpty(txtTitle.Text))
            {
                var newBook = new Book { BookTitle = txtTitle.Text, ISBN = txtISBN.Text };
                var db = new SQLiteConnection(filePath);
                db.Insert(newBook);

                alertTitle = "Success";
                alertMessage = string.Format("Book ID: {0} with Title: {1} has been succesfully added!", newBook.BookId, newBook.BookTitle);
                PopulateListView();
            }
            else

            {
                alertTitle = "Failed";
                alertMessage = "Enter a valid Book Title";
            }
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle(alertTitle);
            alert.SetMessage(alertMessage);
            alert.SetPositiveButton("OK", (senderAlert, args) =>
            {
                Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();

            });
            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                Toast.MakeText(this, "Cancelled", ToastLength.Short).Show();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }
        
    }
}
 

    


