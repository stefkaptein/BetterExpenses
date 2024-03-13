# Calculator worker

## Payment types

### BUNQ
This could be an internal payment between two Bunq accounts. 
For example, from another Bunq user to one of your sub-accounts.

#### Subtypes

###### REQUEST
A direct debit request, for example for recurring payments such as subscriptions and automatic bill payments. 
Also Bunq payment requests from other users fall under this category.

###### PAYMENT
A payment to or from another Bunq account, also your own accounts.

###### BILLING
Billing payment to Bunq, for example your monthly bill or investment fee.

#### MASTERCARD
A payment with one of your cards. Could be at a store, but could also be an online (recurrent) transaction.

#### EBA_SCT
Mainly used for single money transfers, initiated by the payer (sender) of the transaction. A planned transaction, 
like a monthly planned rent payment for example, is also of this type.

###### SCT
This is the only subtype this type has.

#### EBA_SDD
Mainly used for recurring payments, such as subscriptions or monthly bill payments.

#### IDEAL
IDeal transaction.

#### SWIFT
Similar to EBA, however, these payments are not exclusive to the EURO zone.

#### SAVINGS
Movement of funds to a savings account.

#### FIS
FIS payments are similar to SWIFT payments, meaning that they are world wide.

### Payment subtypes
- PAYMENT
- WITHDRAWAL
- REVERSAL
- REQUEST
- BILLING
- SCT
- SDD
- NLO