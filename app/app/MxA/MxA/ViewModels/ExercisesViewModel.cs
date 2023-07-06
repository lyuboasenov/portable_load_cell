﻿using MxA.Database.Models;
using MxA.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MxA.ViewModels {
   public class ExercisesViewModel : BaseViewModel {
      public ObservableCollection<Exercise> Items { get; } = new ObservableCollection<Exercise>();
      public Exercise SelectedItem { get; set; }
      public Command LoadItemsCommand { get; }
      public string SearchTerm { get; set; }

      public ExercisesViewModel() {
         Title = "Exercises";
         LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

         OnAppearing();
      }

      private DateTime _lastSearchTermChange;
      public void OnSearchTermChanged() {
         Task.Run(async () => {
            var last = _lastSearchTermChange = DateTime.Now;
            await Task.Delay(100);
            if (_lastSearchTermChange == last) {
               LoadItemsCommand?.Execute(null);
            }
         });
      }

      async Task ExecuteLoadItemsCommand() {
         IsBusy = true;

         try {
            Items.Clear();
            var items = await DataStore.Exercises.GetItemsAsync();
            items = items.Where(w =>
               string.IsNullOrEmpty(SearchTerm) ||
               w.Description?.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
               w.Name.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
            foreach (var item in items) {
               Items.Add(item);
            }
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         } finally {
            IsBusy = false;
         }
      }

      public void OnAppearing() {
         IsBusy = true;
         SelectedItem = null;
         LoadItemsCommand?.Execute(null);
      }

      public void OnSelectedItemChanged() {
         OnItemSelected(SelectedItem);
      }

      async void OnItemSelected(Exercise item) {
         if (item == null)
            return;

         // This will push the ItemDetailPage onto the navigation stack
         // await Shell.Current.GoToAsync($"{nameof(TimerPage)}?{nameof(TimerViewModel.ActivityId)}={item.Id}");
         await Shell.Current.GoToAsync($"{nameof(ExercisePage)}?{nameof(ExerciseViewModel.ExerciseId)}={item.Id}");
      }
   }
}