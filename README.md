# FxBlotterExcersize

I know this compiles in VS2017 with Framework 4.6 – you are free to use whatever version of .NET / IDE you prefer.  Unfortunately, I can’t package the RX dependencies in with the code because of Lloyds’ restrictions on emailing binaries but it shouldn’t be too hard to fix them up (I suggest using NuGet).
This is the start of a simple blotter intended to display live Foreign Exchange prices.  The user should be able to enter a currency pair and see the live updating price for that pair.  Unfortunately a few things are not working correctly.  Try to fix them:
Price doesn’t update.
Although you can see new prices coming through in the log window (the attached Console window) they are not updating on the blotter.  See if you can fix that.

Price can be overwritten in the GUI.
It’s possible for the user to edit the price in the blotter and that changes the source price.  That shouldn’t happen, the price in the GUI should be read-only.

Adding the same currency more than once gets different values.
If the user adds the same currency pair again we see different prices against it.  Why is that happening?  Please fix it.

Invalid currency pair causes crash.
If the currency pair is not valid the blotter shuts down.  Let’s see if we can handle that a bit better.

Implement & use the IObservable interface of PriceSource.
Someone has started implementing IObservable on PriceSource and they’ve added Reactive Extensions as a dependency.  See if you can make use of that.

If you find any other bugs, feel free to document them and/or clean them up.
It should not be necessary to make any changes in the XAML/WPF (though you’re welcome to if you want to, of course).
Please describe what you do (briefly!) and why you made the decisions you made.  Don’t spend too long on this and don’t worry if you don’t complete everything.  We’re not looking for a perfect solution but rather just looking to get an idea of your experience with C# and have something to discuss at interview.  
