# DynamicData-example

DynamicData-example is my first attempt to work with [DynamicData](https://github.com/RolandPheasant/DynamicData). 

This is just the beginning and a lot of my code is wrongly written, but hope that it will get better with time.

The biggest problem that needs to be understood and made better is concurrency.

Ex. In [Dynamic.Trader](https://github.com/RolandPheasant/Dynamic.Trader) Roland is updating _tradesSource(ISourceCache) with new quotes, but in my case, when update ISourceCache with new quotes and get in to the loop, due to too many dependencies.

My chat with Roland (author of DynamicData library) about making this example better you can find on [DynamicData Issues](https://github.com/RolandPheasant/DynamicData/issues/48) page.

Work in progress, but I am sure I will get in right in couple of weeks’ time.
