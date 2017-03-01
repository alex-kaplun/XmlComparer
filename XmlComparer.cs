public bool CheckXElementsForEquality(XElement root1, XElement root2, List<String> errorsString, String attributeToStopAt = "")
{
	if (XNode.DeepEquals(root1, root2))
	{
		return false;
	}

	if (!root1.HasElements && !root2.HasElements)
	{
		return XElementsEqual(root1, root2, errorsString, attributeToStopAt); // Which is always false
	}

	var elements1 = root1.Elements().ToList();
	var elements2 = root2.Elements().ToList();

	foreach (var el1 in elements1)
	{
		var el2 = elements2.FirstOrDefault(e => XNode.DeepEquals(e, el1));
		if (el2 != null)
		{
			try
			{
				el2.Remove();
				el1.Remove();
			}
			catch (InvalidOperationException)
			{
				// do nothing
			}
		}
	}

	elements1 = root1.Elements().ToList();
	elements2 = root2.Elements().ToList();

	foreach (var el1 in elements1)
	{
		var el2 = elements2.FirstOrDefault(e => String.Compare(
			 GetElementStringWithAttributes(e), GetElementStringWithAttributes(el1)) == 0);
		if (el2 == null)
		{
			errorsString.Add(String.Format("Element {0} is missing from input 2",
											GetElementStringWithParents(el1, attributeToStopAt)));
			continue;
		}
		CheckXElementsForEquality(el1, el2, errorsString, attributeToStopAt);
	}
	foreach (var el2 in elements2)
	{
		var el1 = elements1.FirstOrDefault(e => String.Compare(
			 GetElementStringWithAttributes(e), GetElementStringWithAttributes(el2)) == 0);
		if (el1 == null)
		{
			errorsString.Add(String.Format("Element {0} is missing from input 1",
											GetElementStringWithParents(el2)));
			continue;
		}
	}
	return true;
}

private bool XElementsEqual(XElement x, XElement y, List<string> differences, String attributeToStopAt = "")
{
	if (x.HasAttributes && y.HasAttributes)
	{
		var xAttrs = x.Attributes();
		var yAttrs = y.Attributes();
		if (xAttrs.Count() != yAttrs.Count())
		{
			differences.Add(String.Format("Attributes are different for {0}: was {1}, now {2}",
			GetElementStringWithParents(x), GetElementStringWithAttributes(x), GetElementStringWithAttributes(y)));
			return false;
		}
		foreach (var xAttr in xAttrs)
		{
			var yAttr = xAttrs.Where(a => a.Name == xAttr.Name).FirstOrDefault();
			if (null == yAttr)
			{
				return false;
			}
			if (xAttr.Value != yAttr.Value)
			{
				return false;
			}
		}
		if (x.Value != y.Value)
		{
			differences.Add(String.Format("{0} value changed; was {1}, now {2}",
				 GetElementStringWithParents(x, attributeToStopAt), x.Value, y.Value));
			return false;
		}
	}
	return true;
}

private bool ElementsHaveSameNameAttributesAndSubelements(XElement x, XElement y)
{
	if (x.Name != y.Name)
	{
		return false;
	}
	if (x.HasAttributes || y.HasAttributes)
	{
		var xAttrs = x.Attributes();
		var yAttrs = y.Attributes();

		foreach (var xattr in xAttrs)
		{
			if (yAttrs.FirstOrDefault(a => (a.Name == xattr.Name) && (a.Value == xattr.Value)) == null)
			{
				return false;
			}
		}
		foreach (var yattr in yAttrs)
		{
			if (xAttrs.FirstOrDefault(a => (a.Name == yattr.Name) && (a.Value == yattr.Value)) == null)
			{
				return false;
			}
		}
	}

	if (x.HasElements || y.HasElements)
	{
		var xElems = x.Elements();
		var yElems = y.Elements();

		foreach (var xelem in xElems)
		{
			if (yElems.FirstOrDefault(e => e.Name == xelem.Name) == null)
			{
				return false;
			}
		}
		foreach (var yelem in yElems)
		{
			if (xElems.FirstOrDefault(e => e.Name == yelem.Name) == null)
			{
				return false;
			}
		}
	}

	return true;
}

private String GetElementStringWithAttributes(XElement e)
{
	String result = String.Format("<{0}", e.Name);
	if (e.HasAttributes)
	{
		foreach (var attr in e.Attributes())
		{
			result += String.Format(" {0}=\"{1}\"", attr.Name, attr.Value);
		}
	}
	result += ">";
	return result;
}

private string GetElementStringWithParents(XElement e, String attributeToStopAt = "", int depth = 0)
{
	String result = String.Format("{0}", GetElementStringWithAttributes(e));
	var parent = e.Parent;

	if (parent != null)
	{

		if (!String.IsNullOrEmpty(attributeToStopAt) && parent.HasAttributes && parent.Attribute(attributeToStopAt) != null)
		{
			result = result.Insert(0, GetElementStringWithAttributes(parent) + "/");
		}
		else
		{
			result = result.Insert(0, GetElementStringWithParents(parent, attributeToStopAt) + "/");
		}
	}
	return result;
}
