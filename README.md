Blotter namespace
The blotter namespace includes classes that are responsible for the presentation of the Fx Boltter app. 
The Fx blotter app allows the user to enter currency pairs and if the currency pair has a registered price source in the price source cache 
it will present live randomly simulated Fx price for that currency pair. 
An Fx currency pair can be entered more than once and the price must match (it is derived from the same price source). 

The original app did not have a good seperation of concerns having some code in the view in Blotter.xaml.cs and did not use an Mvvm pattern and the price source observable was not implemented. it used task framework and a timer which are not necessary when you use RX. I therefor re-wrote the code in of the Price source and seperated the dependency of the BlotterRow model from being directly linked to the price source.

I re-wrote the wpf app while preserving the original behaviour of the blotter , the initial default currency pairs , their default prices 
and the supported available currency pairs and the default null blotter rows which I assume are there to allow entering new currency pairs. 

The price source cache allows to reduce memory when multiple requests for the same pair are requested. 
In a real app the price source cache will be a singleton which will emit events to a topic in a message bus allowing multiple subscribers to receive price updates from the topic. 

BlotterViewModel
BlotterViewModel is a class that manages the data and behavior of a blotter view, which displays real-time prices for various currency pairs. The view model subscribes to a price source cache and updates the table with the latest prices for each currency pair. It also allows the user to add or remove rows from the table and change the currency pairs displayed in each row.

Dependencies
The  BlotterViewModel class depends on the following namespaces and libraries:
Blotter.Models : provides the BlotterRow  class used to represent a row in the blotter view.
PriceSupplier : provides the IPriceSourceCache  interface used to subscribe to real-time price updates for various currency pairs.
System.Reactive.Linq : provides reactive programming extension methods used to subscribe to price updates and throttle the rate of updates.
System.Reactive.Disposables: provides the CompositeDisposable class used to manage multiple subscriptions and disposing them
Usage To use the BlotterViewModel class, you need to create an instance of it and pass an 
IPriceSourceCache  object as input. You can also optionally specify a uiUpdateInterval parameter to control how often the view should be updated with the latest prices. Its not currently supported by the UI but sould be easy to implement should it be required.

var priceSourceCache = new MyPriceSourceCache();
var blotterViewModel = new BlotterViewModel(priceSourceCache, uiUpdateInterval: 100);
Once you have created the view model, you can bind it to a view that displays the blotter data. The view should bind to the 
BlotterViewModelRows property of the view model, which is an ObservableCollection  of BlotterRowViewModel objects. Each 
BlotterRowViewModel object represents a row in the blotter view and contains properties for the currency pair, bid price, ask price, and other data.

 <Grid>
        <DataGrid x:Name="Grid" AutoGenerateColumns="False" ItemsSource="{Binding BlotterViewModelRows}">
            <DataGrid.Columns>
                <DataGridTextColumn Header="Currency Pair" Binding="{Binding CurrencyPair}" IsReadOnly="False"/>
                <DataGridTextColumn Header="Price" Binding="{Binding Price, Mode=OneWay}"  ElementStyle="{StaticResource CollapsedIfZero}"/>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>

The view can also interact with the view model by adding or removing rows or changing the currency pairs displayed in each row. To add a new row, the view should add a new BlotterRowViewModel  object to the BlotterViewModelRows collection. To remove a row, the view should remove the corresponding BlotterRowViewModel  object from the collection. To change the currency pair displayed in a row, the view should set the 
CurrencyPair property of the corresponding BlotterRowViewModel object.

Price Supplier
This reactive code provides a Price Supplier that allows subscribing to currency pair prices. It includes a PriceSourceCache class that implements the IPriceSourceCache interface and uses a dictionary to cache the prices of currency pairs. It also includes a static dictionary of available currency pairs and their default initial prices.

The PriceSourceCache class takes a function that creates an IPriceSource object (to improve its testability) and uses it to subscribe to the price updates of a currency pair. If the currency pair is not in the cache, it creates a new BehaviorSubject object with the default initial price and subscribes to the price updates of the currency pair. It then adds the BehaviorSubject object to the cache and returns it as an IObservable object.

The PriceSourceCache class also implements the IDisposable interface to dispose of all the BehaviorSubject objects in the cache when the object is disposed.

The PriceSource class implements the IPriceSource interface and generates random price updates for a given currency pair. It takes a string representing the currency pair and a decimal representing the initial price. If the currency pair is USDJPY, it sets the rounding to 2. It then creates a new CancellationDisposable object and a CompositeDisposable object to manage the subscriptions. It uses the Observable.Create method to create a new observable sequence that generates random price updates for the currency pair. It rounds the price to the specified number of decimal places and adds it to the sequence using the OnNext method of the observer. It then waits for a random amount of time between 50 and 2000 milliseconds before generating the next price update. If the sequence is cancelled, it completes the sequence using the OnCompleted method of the observer. It then subscribes to the sequence using the Subscribe method and adds the subscription to the composite disposable. It also adds the cancellation disposable to the composite disposable and returns it as the result of the Subscribe method.

This code uses the System, System.Reactive.Disposables, System.Reactive.Linq, and System.Threading.Tasks namespaces.
License
This code is released under the MIT License. It should be used for educational purposes only. 

PriceSupplier Unit Tests
This repository contains unit tests for the 
PriceSupplier
 project. The tests are written using the Microsoft Visual Studio testing framework.

Getting Started
To run the tests, you will need to have Microsoft Visual Studio installed on your machine. You can download Visual Studio from the official website.

Once you have installed Visual Studio, you can open the solution file (PriceSupplier.sln) in Visual Studio and run the tests using the Test Explorer.

Running the Tests
To run the tests, follow these steps:

Open the solution file (PriceSupplier.sln) in Visual Studio.
Open the Test Explorer window by selecting Test > Windows > Test Explorer from the menu bar.
In the Test Explorer window, select the tests you want to run.
Click the Run button to run the selected tests.
You can also run the tests from the command line using the dotnet test command. To run all the tests, navigate to the root directory of the repository and run the following command:
dotnet test

Test Cases
The following test cases are included in this repository:

PriceSourceCache_Subscribe_ReturnsObservable : Tests that the Subscribe  method of the PriceSourceCache class returns an observable.
PriceSourceCache_Subscribe_ReturnsSameObservableForSameCurrencyPair : Tests that the Subscribe  method of the PriceSourceCache  class returns the same observable for the same currency pair.
PriceSourceCache_Subscribe_ReturnsDifferentObservableForDifferentCurrencyPair : Tests that the Subscribe  method of the PriceSourceCache  class returns a different observable for different currency pairs.
PriceSourceCache_Subscribe_ReturnsObservableWithCorrectCurrencyPair : Tests that the observable returned by the  Subscribe method of the PriceSourceCache  class has the correct currency pair.
PriceSourceCache_Dispose_DisposesAllObservables: Tests that the Dispose  method of the PriceSourceCache  class disposes all the observables returned by the Subscribe method.

The code and the documentation was written by Avi Ben-Margi with the help of ChatGPT 4.0 and its VsCode extension
