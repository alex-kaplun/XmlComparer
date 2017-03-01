# XmlComparer
A tool to compare two XMLs ignoring order of subnodes

How to use:

using System.Xml.Linq;

List<String> errors = new List<string>();

XDocument doc1 = XDocument.Load(_XmlFile1);
XDocument doc2 = XDocument.Load(_XmlFile2);

var root1 = doc1.Root;
var root2 = doc2.Root;

CheckXElementsForEquality(root1, root2, errors, stopAttribute);

if (errors.Count == 0)
{
  Console.WriteLine("XMLs are equal");
}
foreach (var errorstring in errors)
{
  Console.WriteLine(errorstring);
}
