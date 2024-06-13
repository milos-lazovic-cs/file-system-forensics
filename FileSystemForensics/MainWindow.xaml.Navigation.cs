using FileSystemForensics.Services.Interfaces;
using FileSystemForensics.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemForensics;


public partial class MainWindow : Window, INavigation
{

    #region Controls Handlers

    private void NavigationView_Loaded(object sender, RoutedEventArgs e)
    {
        // Navigates, but does not update the Menu.
        ContentFrame.Navigate(typeof(HomePage));

        SetCurrentNavigationViewItem(GetNavigationViewItems(typeof(HomePage)).First());
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        SetCurrentNavigationViewItem(args.SelectedItemContainer as NavigationViewItem);
    }



    #endregion


    #region Interface Methods

    public List<NavigationViewItem> GetNavigationViewItems()
    {
        var result = new List<NavigationViewItem>();
        var items = NavigationView.MenuItems.Select(item => (NavigationViewItem)item).ToList();
        items.AddRange(NavigationView.FooterMenuItems.Select(item => (NavigationViewItem)item));

        result.AddRange(items);
        foreach (NavigationViewItem mainItem in items)
        {
            result.AddRange(mainItem.MenuItems.Select(i => (NavigationViewItem)i));
        }

        return result;
    }

    public List<NavigationViewItem> GetNavigationViewItems(Type type)
    {
        return GetNavigationViewItems().Where(item => item.Tag.ToString() == type.FullName).ToList();
    }

    public List<NavigationViewItem> GetNavigationViewItem(Type type, string title)
    {
        return GetNavigationViewItems(type).Where(item => item.Content.ToString() == title).ToList();
    }

    public NavigationViewItem GetCurrentNavigationViewItem()
    {
        return NavigationView.SelectedItem as NavigationViewItem;
    }

    public void SetCurrentNavigationViewItem(NavigationViewItem item)
    {
        if (item == null)
            return;

        if (item.Tag == null)
            return;

        ContentFrame.Navigate(Type.GetType(item.Tag.ToString()), item.Content);
        NavigationView.Header = item.Content;
        NavigationView.SelectedItem = item;
    }

    public void SetCurrentPage(Type type)
    {
        ContentFrame.Navigate(type);
    }

    #endregion


}


