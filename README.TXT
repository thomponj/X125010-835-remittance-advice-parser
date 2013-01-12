X125010-835-remittance-advice-parser

author:  thomponj (Jeremy Whitwam)

Parses an X12 835 remittance advice file (standard USA health care file format) to the billing provider level.  
A new file for each different billing provider loop is created, with the billing provider name in the filename.
Original file is read, but not manipulated or changed in any way.
File name format is: originalname_billingprovider_counter.originalextension

TODO:
- cleanup code by fixing predetermined array size of 1000
- verify SE segment counts
