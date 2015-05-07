using System;

namespace RestAssure.LongList
{
    public class HtmlStringParser
    {
        public HtmlStringParser(string allText)
        {
            _allText = allText;
        }

        private readonly string _allText;
        private int _iLinkUriStart;
        private int _iLinkUriEnd;
        private string _sGrouping;

        public LongListItem ParseItem()
        {
            if (string.IsNullOrEmpty(_allText))
            {
                return null;
            }

            int iGroupingStart = _allText.IndexOf("$_Section:", _iLinkUriEnd, StringComparison.CurrentCultureIgnoreCase);
            int iItemStart = _allText.IndexOf("$_Item:", _iLinkUriEnd, StringComparison.CurrentCultureIgnoreCase);
            int iItemEnd;

            if (iItemStart == -1)
            {
                return null;
            }

            if (iItemStart < iGroupingStart || iGroupingStart < 0)
            {
                iItemStart = _allText.IndexOf("$_Item:", _iLinkUriEnd, StringComparison.CurrentCultureIgnoreCase) + 7;
                iItemEnd = _allText.IndexOf(";", iItemStart, StringComparison.CurrentCultureIgnoreCase);
            }
            else
            {
                iGroupingStart = _allText.IndexOf("$_Section:", _iLinkUriEnd, StringComparison.CurrentCultureIgnoreCase) + 10;
                int iGroupingEnd = _allText.IndexOf(";", iGroupingStart, StringComparison.CurrentCultureIgnoreCase);

                iItemStart = _allText.IndexOf("$_Item:", iGroupingEnd, StringComparison.CurrentCultureIgnoreCase) + 7;
                iItemEnd = _allText.IndexOf(";", iItemStart, StringComparison.CurrentCultureIgnoreCase);
                _sGrouping = _allText.Substring(iGroupingStart, iGroupingEnd - iGroupingStart).Trim();
            }

            int iImageUriStart = _allText.IndexOf("$_Thumbnail:", iItemEnd, StringComparison.CurrentCultureIgnoreCase);
            int iImageUriEnd = iItemEnd;
            string sImageUri = "";
            if (iImageUriStart > 0)
            {
                iImageUriStart = _allText.IndexOf("src=\"", iImageUriStart, StringComparison.CurrentCultureIgnoreCase) + 5;
                iImageUriEnd = _allText.IndexOf("\"", iImageUriStart, StringComparison.CurrentCultureIgnoreCase);
                sImageUri = _allText.Substring(iImageUriStart, iImageUriEnd - iImageUriStart).Trim();
            }

            _iLinkUriStart = _allText.IndexOf("$_Link:", iImageUriEnd, StringComparison.CurrentCultureIgnoreCase);
            _iLinkUriStart = _allText.IndexOf("href=\"", _iLinkUriStart, StringComparison.CurrentCultureIgnoreCase) + 6;
            _iLinkUriEnd = _allText.IndexOf("\"", _iLinkUriStart, StringComparison.CurrentCultureIgnoreCase);

            string sTitle = _allText.Substring(iItemStart, iItemEnd - iItemStart).Trim();
            string sLinkUri = _allText.Substring(_iLinkUriStart, _iLinkUriEnd - _iLinkUriStart).Trim();

            return new LongListItem(_sGrouping, sTitle, sImageUri, sLinkUri);
        }
    }
}