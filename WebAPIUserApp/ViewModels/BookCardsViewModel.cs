using System.ComponentModel;
using System.Windows;

namespace WebAPIUserApp.ViewModels
{
    class BookCardsViewModel : DependencyObject
    {
        public ICollectionView BookCards
        {
            get { return (ICollectionView)GetValue(BookCardsProperty); }
            set { SetValue(BookCardsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BookCards.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BookCardsProperty =
            DependencyProperty.Register("BookCards", typeof(ICollectionView), typeof(BookCardsViewModel), new PropertyMetadata(null));

        public BookCardsViewModel()
        {
            
        }

    }
}
