using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XamarinStripe.Forms.ViewModels {
  public class ViewModelBase : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    protected bool SetProperty<T>(ref T backingStore, T value,
      Action onChanged = null,
      [CallerMemberName] string propertyName = "") {
      if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;

      backingStore = value;
      onChanged?.Invoke();
      OnPropertyChanged(propertyName);
      return true;
    }
  }
}