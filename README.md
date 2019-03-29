# Xsd Test
#### Just a tiny project for test adidas template XSD file validate.
 
 
## V1.1 using guide
```
XsdTest.exe [XML FILE] [PURPOSE]
```
* [XML FILE] : Your XML File.
* [PURPOSE] : adidas's XML purpose, **case sensitive**.  

For example: `XsdTest.exe test.xml Confirmation`  
  

## Methods Explanation
- **XmlToGeneric**   
Convert XML to Model (better way).
- **XmlToObj**   
Convert XML to Object.
- **ObjToXml**   
Convert Object/Model to XML string.
- **XmlValidate**   
Using xsd file to vaildate XML format.
- **GetMd5String**   
Get MD5 hash from a string
