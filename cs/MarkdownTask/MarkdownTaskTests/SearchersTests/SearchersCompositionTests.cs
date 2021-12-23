﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MarkdownTask.Tags;
using MarkdownTask.TagSearchers;
using NUnit.Framework;

namespace MarkdownTaskTests.SearchersTests
{
    public class SearchersCompositionTests
    {
        private List<ITagSearcher> searchers;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            searchers = new List<ITagSearcher>
            {
                new HeaderTagSearcher(),
                new ItalicTagSearcher(),
                new StrongTagSearcher()
            };
        }

        [Test]
        public void CompositionTest(
            [ValueSource(nameof(TestCasesForCompositeTest))]
            Tuple<string, List<Tag>> testCases)
        {
            var mdText = testCases.Item1;
            var expectedResult = testCases.Item2;

            var actualResult = GetTagsComposition(mdText);

            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        private List<Tag> GetTagsComposition(string mdText)
        {
            return searchers
                .SelectMany(searcher => searcher
                    .SearchForTags(mdText))
                .OrderBy(tag => tag.StartsAt)
                .ToList();
        }

        private static IEnumerable<Tuple<string, List<Tag>>> TestCasesForCompositeTest()
        {
            yield return Tuple.Create("_some_ __text__", new List<Tag>
            {
                new Tag(0, 6, TagType.Italic),
                new Tag(7, 8, TagType.Strong)
            });

            yield return Tuple.Create("# __new__ _paragraph_", new List<Tag>
            {
                new Tag(0, 21, TagType.Header),
                new Tag(2, 7, TagType.Strong),
                new Tag(10, 11, TagType.Italic)
            });

            yield return Tuple.Create("# __first__ paragraph\n\n# _second_ paragraph", new List<Tag>
            {
                new Tag(0, 22, TagType.Header),
                new Tag(2, 9, TagType.Strong),
                new Tag(23, 20, TagType.Header),
                new Tag(25, 8, TagType.Italic)
            });
        }
    }
}