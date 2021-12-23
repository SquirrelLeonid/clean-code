﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using MarkdownTask.Tags;
using MarkdownTask.TagSearchers;
using NUnit.Framework;

namespace MarkdownTaskTests.SearchersTests
{
    public class StrongTagSearcherTests
    {
        private ITagSearcher searcher;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            searcher = new StrongTagSearcher();
        }

        [TestCase("text")]
        [TestCase("")]
        [TestCase("text_")]
        [TestCase("_text_")]
        [TestCase("__text")]
        public void Searcher_ShouldNotDefineAnyTags(string mdText)
        {
            var actualResult = searcher.SearchForTags(mdText);
            actualResult.Should().HaveCount(0);
        }

        [TestCase("__ text__")]
        [TestCase("__text __")]
        public void Searcher_ShouldNotDefineTag_WhenSpaceBeforeOrAfterTag(string mdText)
        {
            var actualResult = searcher.SearchForTags(mdText);
            actualResult.Should().HaveCount(0);
        }

        [Test]
        public void Searcher_ShouldNotDefineTag_WhenTagInDifferentWords()
        {
            var mdText = "so__me te__xt";
            var actualResult = searcher.SearchForTags(mdText);
            actualResult.Should().HaveCount(0);
        }

        [TestCase("__")]
        [TestCase("___")]
        [TestCase("____")]
        public void Searcher_ShouldNotDefineTag_IfTagNotWrapSomething(string mdText)
        {
            var actualResult = searcher.SearchForTags(mdText);
            actualResult.Should().HaveCount(0);
        }

        [TestCase("text__1__")]
        [TestCase("text__1__2")]
        public void Searcher_ShouldNotDefineTag_IfTagWrapNumbers(string mdText)
        {
            var actualResult = searcher.SearchForTags(mdText);
            actualResult.Should().HaveCount(0);
        }

        [Test]
        public void Searcher_ShouldCorrectlyDefineTag_InDifferentPartsOfWord(
            [ValueSource(nameof(CasesForStrongTag))]
            Tuple<string, List<Tag>> testCase)
        {
            var mdText = testCase.Item1;
            var expectedResult = testCase.Item2;

            var actualResult = searcher.SearchForTags(mdText);
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Searcher_ShouldCorrectlyDefineTags_WhenMoreThatOneTag(
            [ValueSource(nameof(CasesForMultipleStrongTag))]
            Tuple<string, List<Tag>> testCase)
        {
            var mdText = testCase.Item1;
            var expectedResult = testCase.Item2;

            var actualResult = searcher.SearchForTags(mdText);
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Searcher_ShouldNotDefineTag_IfTagIsOpened(
            [ValueSource(nameof(CasesForOpenedStrongTag))]
            Tuple<string, List<Tag>> testCase)
        {
            var mdText = testCase.Item1;
            var expectedResult = testCase.Item2;

            var actualResult = searcher.SearchForTags(mdText);
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        private static IEnumerable<Tuple<string, List<Tag>>> CasesForStrongTag()
        {
            yield return Tuple.Create("__text__", new List<Tag>
            {
                new Tag(0, 8, TagType.Strong)
            });

            yield return Tuple.Create("__te__xt", new List<Tag>
            {
                new Tag(0, 6, TagType.Strong)
            });

            yield return Tuple.Create("t__ex__t", new List<Tag>
            {
                new Tag(1, 6, TagType.Strong)
            });

            yield return Tuple.Create("te__xt__", new List<Tag>
            {
                new Tag(2, 6, TagType.Strong)
            });
        }

        private static IEnumerable<Tuple<string, List<Tag>>> CasesForMultipleStrongTag()
        {
            yield return Tuple.Create("__so__me te__xt__", new List<Tag>
            {
                new Tag(0, 6, TagType.Strong),
                new Tag(11, 6, TagType.Strong)
            });

            yield return Tuple.Create("__some__ __text__", new List<Tag>
            {
                new Tag(0, 8, TagType.Strong),
                new Tag(9, 8, TagType.Strong)
            });
        }

        private static IEnumerable<Tuple<string, List<Tag>>> CasesForOpenedStrongTag()
        {
            yield return Tuple.Create("__te__x__t", new List<Tag>
            {
                new Tag(0, 6, TagType.Strong)
            });

            yield return Tuple.Create("__some__ __text", new List<Tag>
            {
                new Tag(0, 8, TagType.Strong)
            });
        }
    }
}