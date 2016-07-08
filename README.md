# DynamicData-example

DynamicData-example is my first attempt to work with [DynamicData](https://github.com/RolandPheasant/DynamicData). 
Intention of this example is to create real time trading limit management back-end for margin trading with use of DynamiData.
Example is Calculating:
- P&L of the trades
- Agreegated P&L of the customer
- Margin Used for open trades based on Net Open Possition per Currency
- Net Open Possition (NOP) per Currency per Customer
- Open Possition per Currency Pair per Customer
If Margin is not enought, system is closing the possition of the customer.
Missing Part is the limit check procedure if new trade requested by the customer can be executed or not.

This example should prove that DynamicData is suitable for this kind of system.
Crusial problem that is still not adreessed correctly is concurency and source of truth for particular actions:
ex. New NOP calculation is done, we update client data with right margin, calculation might be  already wrong, because another trade has come in the meanwhile and then accepting new trade is wrong.

----

This is the beginning and a lot of my code is wrongly written, but hope that it will get better with time.
Work in progress, but I am sure I will get in right in couple of weeks’ time.


My chat with Roland (author of DynamicData library) about making this example better you can find on [DynamicData Issues](https://github.com/RolandPheasant/DynamicData/issues/48) page.


