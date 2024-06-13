﻿using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemForensics.Services.Interfaces;

public interface INavigation
{
    NavigationViewItem GetCurrentNavigationViewItem();
    List<NavigationViewItem> GetNavigationViewItems();
    List<NavigationViewItem> GetNavigationViewItems(Type type);
    List<NavigationViewItem> GetNavigationViewItem(Type type, string title);
    void SetCurrentNavigationViewItem(NavigationViewItem item);
    void SetCurrentPage(Type type);
}
