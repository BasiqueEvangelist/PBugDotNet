// SPDX-FileCopyrightText: Alexandre Mutel
// SPDX-License-Identifier: BSD-2-Clause

// Based on https://github.com/xoofx/markdig/blob/main/src/Markdig/Extensions/JiraLinks/JiraLinkInlineParser.cs.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace PBug.Utils;

public class PBugLink : LinkInline
{
    public PBugLink()
    {
        IsClosed = true;
    }

    public StringSlice ProjectId { get; set; }

    public StringSlice IssueId { get; set; }
}

public class PBugLinkInlineParser(IHttpContextAccessor ctxAccessor, LinkGenerator linkGenerator) : InlineParser
{
    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        // Allow preceding whitespace or `(`
        var pc = slice.PeekCharExtra(-1);
        if (!pc.IsWhiteSpaceOrZero() && pc != '(')
        {
            return false; 
        }

        var current = slice.CurrentChar;

        var startKey = slice.Start;
        var endKey = slice.Start;

        // the first character of the key can not be a digit.
        if (current.IsDigit())
        {
            return false;
        }

        // read as many uppercase characters or digits as required - project key
        while (current.IsAlphaUpper() || current.IsDigit())
        {
            endKey = slice.Start;
            current = slice.NextChar();
        }

        //require a '-' between key and issue number
        if (!current.Equals('-'))
        {
            return false;
        }

        current = slice.NextChar(); // skip -

        //read as many numbers as required - issue number
        if (!current.IsDigit())
        {
            return false;
        }

        var startIssue = slice.Start;
        var endIssue = slice.Start;

        while (current.IsDigit()) 
        {
            endIssue = slice.Start;
            current = slice.NextChar();
        }

        if (!current.IsWhiteSpaceOrZero() && current != ')') //can be followed only by a whitespace or `)`
        {
            return false;
        }

        int spanStart = processor.GetSourcePosition(startKey, out int line, out int column);
        var link = new PBugLink() //create the link at the relevant position
        {
            Span = new SourceSpan(spanStart, spanStart + (endIssue - startKey)),
            Line = line,
            Column = column,
            IssueId = new StringSlice(slice.Text, startIssue, endIssue),
            ProjectId = new StringSlice(slice.Text, startKey, endKey),
        };

        link.Url = linkGenerator.GetPathByAction(ctxAccessor.HttpContext!, "ViewTalk", "Issue", values: new {});

        link.AppendChild(new LiteralInline($"{link.ProjectId.AsSpan()}-{link.IssueId.AsSpan()}")
        {
            Span = link.Span,
            Line = line,
            Column = column,
        });

        processor.Inline = link;

        return true;
    }
}