using CommonEntities;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WebAPIUserApp.ViewModels;

namespace WebAPIUserApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddBtn.IsEnabled = false;
            EditBtn.IsEnabled = false;
            DeleteBtn.IsEnabled = false;
        }

        private async Task Refresh()
        {
            AddBtn.IsEnabled = true;
            EditBtn.IsEnabled = true;
            DeleteBtn.IsEnabled = true;
            try
            {
                var bookCards = await ConnectionModule.GetAllBookCardsAsync();
                if(bookCards.Count == 0)
                {
                    MessageBox.Show("Нет записей");
                }
                DataContext = new BookCardsViewModel()
                {
                    BookCards = CollectionViewSource.GetDefaultView(bookCards)
                };
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            InputWindow inputWindow = new InputWindow(new BookCard());
            inputWindow.ShowDialog();
            if (inputWindow.BookCard == null)
            {
                return;
            }
            var response = await ConnectionModule.PostBookCardAsync(inputWindow.BookCard);
            if(response.ToString() == "OK")
            {
                MessageBox.Show("Успешно!");
            }
            else
            {
                MessageBox.Show(response.ToString());
            }
            await Refresh();
        }

        private async void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }

        private async void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if(BookCardsGrid.SelectedItems.Count>1)
            {
                MessageBox.Show("Выберите один элемент");
                return;
            }
            BookCard bookCard = BookCardsGrid.SelectedItem as BookCard;
            if (bookCard == null)
            {
                MessageBox.Show("Сначала выберите элемент.");
                return;
            }
            InputWindow inputWindow = new InputWindow(bookCard);
            inputWindow.ShowDialog();
            if (inputWindow.BookCard == null)
            {
                return;
            }
            var response = await ConnectionModule.PutBookCard(inputWindow.BookCard);
            await Refresh();
            if (response.ToString() == "OK")
            {
                MessageBox.Show("Успешно!");
            }
            else
            {
                MessageBox.Show(response.ToString());
            }
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            int count = BookCardsGrid.SelectedItems.Count;
            if (count == 0)
            {
                MessageBox.Show("Сначала выберите элементы.");
                return;
            }
            int[] idArray = new int[count];
            try
            {
                int index = 0;
                foreach (var item in BookCardsGrid.SelectedItems)
                {
                    idArray[index++] = (item as BookCard).Id;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка выбора элемента.");
                return;
            }
            var response = await ConnectionModule.DeleteBookCard(idArray);
            await Refresh();
            if (response.ToString() == "OK")
            {
                MessageBox.Show("Успешно!");
            }
            else
            {
                MessageBox.Show(response.ToString());
            };
        }
    }
}
