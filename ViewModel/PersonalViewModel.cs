using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module05Exercise01.Model;
using Module05Exercise01.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mysqlx.Crud;

namespace Module05Exercise01.ViewModel
{
    public class PersonalViewModel : INotifyPropertyChanged
    {
        private readonly PersonalService _personalService;
        public ObservableCollection<Personal> PersonalList { get; set; }
        public ObservableCollection<Personal> FilteredPersonalList { get; set; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterPersonalList();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();

            }
        }

        private Personal _selectedPersonal;

        public Personal SelectedPersonal
        {
            get => _selectedPersonal;
            set
            {
                _selectedPersonal = value;
                if (_selectedPersonal != null)
                {
                    NewPersonalName = _selectedPersonal.Name;
                    NewPersonalAddress = _selectedPersonal.Address;
                    NewPersonalEmail = _selectedPersonal.Email;
                    NewPersonalContactNo = _selectedPersonal.ContactNo;
                    IsPersonSelected = true;
                    IsPersonSelectedAdd = false;
                }
                else
                {
                    IsPersonSelected = false;
                    IsPersonSelectedAdd = true;
                }
                OnPropertyChanged();
            }
        }

        private bool _isPersonSelected;
        public bool IsPersonSelected
        {
            get => _isPersonSelected;
            set
            {
                _isPersonSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isPersonSelectedAdd;
        public bool IsPersonSelectedAdd
        {
            get => _isPersonSelectedAdd;
            set
            {
                _isPersonSelectedAdd = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        // New Personal entry for name, address, email, contactno
        private string _newPersonalName;
        public string NewPersonalName
        {
            get => _newPersonalName;
            set
            {
                _newPersonalName = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalAddress;
        public string NewPersonalAddress
        {
            get => _newPersonalAddress;
            set
            {
                _newPersonalAddress = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalEmail;
        public string NewPersonalEmail
        {
            get => _newPersonalEmail;
            set
            {
                _newPersonalEmail = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalContactNo;
        public string NewPersonalContactNo
        {
            get => _newPersonalContactNo;
            set
            {
                _newPersonalContactNo = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand AddPersonalCommand { get; }
        public ICommand UpdatePersonCommand { get; }
        public ICommand SelectedPersonCommand { get; }
        public ICommand DeletePersonCommand { get; }

        //PersonalViewModel Constructor

        public PersonalViewModel()
        {
            _personalService = new PersonalService();
            PersonalList = new ObservableCollection<Personal>();
            FilteredPersonalList = new ObservableCollection<Personal>();
            LoadDataCommand = new Command(async () => await LoadData());
            AddPersonalCommand = new Command(async() => await AddPerson());
            UpdatePersonCommand = new Command(async () => await UpdatePerson());
            SelectedPersonCommand = new Command<Personal>(person => SelectedPersonal = person);
            DeletePersonCommand = new Command(async() => 
                                  await DeletePersonal(),
                                  () => SelectedPersonal != null);

            LoadData();
        }

        public async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = "Loading personal data...";
            try
            {
                var personals = await _personalService.GetAllPersonalsAsync();
                PersonalList.Clear();
                foreach (var personal in personals)
                {
                    PersonalList.Add(personal);
                }
                StatusMessage = "Data loaded successfully!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load data: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddPerson()
        {
            if(IsBusy || string.IsNullOrWhiteSpace(NewPersonalName) || string.IsNullOrWhiteSpace(NewPersonalAddress) || string.IsNullOrWhiteSpace(NewPersonalEmail) || string.IsNullOrWhiteSpace(NewPersonalContactNo))
            {
                StatusMessage = "Please fill in all the fields provided before adding an employee.";
                return;
            }
            IsBusy = true;
            StatusMessage = "Adding new employee...";

            try
            {
                var newPersonal = new Personal
                {
                    Name = NewPersonalName,
                    Address = NewPersonalAddress,
                    Email = NewPersonalEmail,
                    ContactNo = NewPersonalContactNo
                };
                var isSuccess = await _personalService.AddPersonalAsync(newPersonal);
                if (isSuccess)
                {
                    NewPersonalName = string.Empty;
                    NewPersonalAddress = string.Empty;
                    NewPersonalEmail = string.Empty;
                    NewPersonalContactNo = string.Empty;
                    StatusMessage = "Employee added successfully!";
                }
                else
                {
                    StatusMessage = "Failed to add the new employee.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed adding employee: {ex.Message}";
            }
            finally
            { 
                IsBusy = false;
                await LoadData();
            }
        }

        private async Task DeletePersonal()
        {
            if (SelectedPersonal == null) return;
            var answer = await Application.Current.MainPage.DisplayAlert("Confirm Delete", $"Are you sure you want to delete {SelectedPersonal.Name}?",
                "Yes", "No");

            if (!answer) return;
            IsBusy = true;
            StatusMessage = "Deleting Employee...";

            try
            {
                var success = await _personalService.DeletePersonalAsync(SelectedPersonal.EmployeeID);
                StatusMessage = success ? "Employee entry deleted successfully." : "Failed to delete employee entry";

                if (success)
                {
                    PersonalList.Remove(SelectedPersonal);
                    SelectedPersonal = null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting the employee entry: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UpdatePerson()
        {
            if (IsBusy ||  SelectedPersonal == null)
            {
                StatusMessage = "Select an employee entry to update";
                return;
            }

            IsBusy = true;
            try
            {
                SelectedPersonal.Name = NewPersonalName;
                SelectedPersonal.Address = NewPersonalAddress;
                SelectedPersonal.Email = NewPersonalEmail;
                SelectedPersonal.ContactNo = NewPersonalContactNo;

                var success = await _personalService.UpdatePersonalAsync(SelectedPersonal);
                StatusMessage = success ? "Employee entry updated successfully." : "Failed to update employee entry.";

                await LoadData();
                ClearFields();
            }

            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }

            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void FilterPersonalList()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredPersonalList.Clear();
                foreach (var person in PersonalList)
                {
                    FilteredPersonalList.Add(person);
                }
            }
            else
            {
                var filtered = PersonalList.Where(p => p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                                         || p.Address.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                                         || p.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                                         || p.ContactNo.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                FilteredPersonalList.Clear();
                foreach (var person in filtered)
                {
                    FilteredPersonalList.Add(person);
                }
            }
        }

        private void ClearFields()
        {
            NewPersonalName = string.Empty;
            NewPersonalAddress = string.Empty;
            NewPersonalEmail = string.Empty;
            NewPersonalContactNo = string.Empty;
        }
    }
}
