using CommonEntities;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WebAPIUserApp
{
    /// <summary>
    /// Логика взаимодействия для InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        private BookCard newState;

        public BookCard BookCard { get; set; }

        private bool photoHasChoosen = false;

        public InputWindow()
        {
            InitializeComponent();
        }

        public InputWindow(BookCard bookCard) : this()
        {
            this.BookCard = null;
            newState = bookCard;
            if (bookCard.Name != "placeholder")
            {
                NameInput.Text = bookCard.Name;
                photoHasChoosen = true;
            }
            try
            {
                BookCardImage.Source = bookCard.BitmapImagePhoto;
            }
            catch (Exception)
            {
                BookCardImage.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\" + ConfigurationManager.AppSettings.Get("defaultPhotoPath")));
            }
        }

        private void BookCardImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Выберите изображение";
            op.Filter = "All supported graphics|*.jpg;*.png|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                newState.BinaryPhoto = null;
                newState.BitmapPhoto = new System.Drawing.Bitmap(op.FileName);
                BookCardImage.Source = new BitmapImage(new Uri(op.FileName));
                photoHasChoosen = true;
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NameInput.Text.Length == 0)
            {
                MessageBox.Show("Введите название книги!");
                return;
            }
            if(photoHasChoosen == false)
            {
                MessageBox.Show("Выберите фото!");
                return;
            }
            newState.Name = NameInput.Text;
            BookCard = newState;
            this.Close();
        }
    }
}
