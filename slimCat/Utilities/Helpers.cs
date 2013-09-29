﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="Justin Kadrovach">
//   Copyright (c) 2013, Justin Kadrovach
//   All rights reserved.
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
//       * Redistributions of source code must retain the above copyright
//         notice, this list of conditions and the following disclaimer.
//       * Redistributions in binary form must reproduce the above copyright
//         notice, this list of conditions and the following disclaimer in the
//         documentation and/or other materials provided with the distribution.
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//   ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//   WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//   DISCLAIMED. IN NO EVENT SHALL JUSTIN KADROVACH BE LIABLE FOR ANY
//   DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//   (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//   LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//   ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//   (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
// <summary>
//   The extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Slimcat.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Timers;
    using System.Web;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    using Models;

    /// <summary>
    ///     The extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The first by id or default.
        /// </summary>
        /// <param name="model">
        ///     The model.
        /// </param>
        /// <param name="ID">
        ///     The id.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        public static T FirstByIdOrDefault<T>(this ICollection<T> model, string ID) where T : ChannelModel
        {
            return model.FirstOrDefault(param => param.Id.Equals(ID, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     The throw if null.
        /// </summary>
        /// <param name="x">
        ///     The x.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static T ThrowIfNull<T>(this T x, string name)
        {
            if (x == null)
            {
                throw new ArgumentNullException(name);
            }

            return x;
        }

        public static void Each<T>(this IEnumerable<T> collection, Action<T> functor)
        {
            foreach (var item in collection)
            {
                functor(item);
            }
        }

        public static string FormatWith(this string toFormat, params object[] args)
        {
            return string.Format(toFormat, args);
        }

        public static void RemoveAt(this BlockCollection collection, int index)
        {
            if (index == -1 || collection.Count == 0)
            {
                return;
            }

            collection.Remove(collection.ElementAt(index));
        }

        public static void AddAt(this BlockCollection collection, int index, Block item)
        {
            if (index == -1)
            {
                return;
            } 

            if (collection.Count == 0)
            {
                collection.Add(item);
                return;
            }

            index = Math.Min(index, collection.Count - 1);

            collection.InsertAfter(collection.ElementAt(index), item);
        }
        #endregion
    }

    /// <summary>
    ///     The static functions.
    /// </summary>
    public static class StaticFunctions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The character is in list.
        /// </summary>
        /// <param name="collection">
        ///     The collection.
        /// </param>
        /// <param name="toFind">
        ///     The to find.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CharacterIsInList(this ICollection<ICharacter> collection, ICharacter toFind)
        {
            return collection.Any(character => character.Name.Equals(toFind.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     Checks if a string contains a term using ordinal string comparison
        /// </summary>
        /// <param name="fullString">
        ///     The full String.
        /// </param>
        /// <param name="checkterm">
        ///     The checkterm.
        /// </param>
        /// <param name="ignoreCase">
        ///     The ignore Case.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool ContainsOrdinal(this string fullString, string checkterm, bool ignoreCase = true)
        {
            StringComparison comparer = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return fullString.IndexOf(checkterm, comparer) >= 0;
        }

        /// <summary>
        ///     Checks if a given collection has a matching word or phrase. Returns the word and its context in a tuple.
        ///     Empty if no match.
        /// </summary>
        /// <param name="fullString">
        ///     The full String.
        /// </param>
        /// <param name="checkAgainst">
        ///     The check Against.
        /// </param>
        /// <returns>
        ///     The <see cref="Tuple" />.
        /// </returns>
        public static Tuple<string, string> FirstMatch(this string fullString, string checkAgainst)
        {
            int startIndex = fullString.IndexOf(checkAgainst, StringComparison.OrdinalIgnoreCase);

            bool hasMatch = false;

            if (startIndex != -1)
            {
                // this checks for if the match is a whole word
                if (startIndex != 0)
                {
                    // this weeds out matches such as 'big man' from matching 'i'
                    char prevChar = fullString[startIndex - 1];
                    hasMatch = char.IsWhiteSpace(prevChar) || char.IsPunctuation(prevChar) && !prevChar.Equals('\'');

                    if (!hasMatch)
                    {
                        return new Tuple<string, string>(string.Empty, string.Empty);

                        // don't need to evaluate further if this failed
                    }
                }

                if (startIndex + checkAgainst.Length < fullString.Length)
                {
                    // this weeds out matches such as 'its' from matching 'i'
                    int nextIndex = startIndex + checkAgainst.Length;
                    char nextChar = fullString[nextIndex];
                    hasMatch = char.IsWhiteSpace(nextChar) || char.IsPunctuation(nextChar);

                    // we only want the ' to match sometimes, such as <match word>'s
                    if (nextChar == '\'' && fullString.Length >= nextIndex++)
                    {
                        nextChar = fullString[nextIndex];
                        hasMatch = char.ToLower(nextChar) == 's';
                    }
                }
            }

            return hasMatch 
                ? new Tuple<string, string>(checkAgainst, GetStringContext(fullString, checkAgainst)) 
                : new Tuple<string, string>(string.Empty, string.Empty);
        }

        /// <summary>
        ///     returns the sentence (ish) around a word
        /// </summary>
        /// <param name="fullContent">
        ///     The full Content.
        /// </param>
        /// <param name="specificWord">
        ///     The specific Word.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetStringContext(string fullContent, string specificWord)
        {
            const int MaxDistance = 150;
            var needle = fullContent.ToLower().IndexOf(specificWord.ToLower(), StringComparison.Ordinal);

            var start = Math.Max(0, needle - MaxDistance);
            var end = Math.Min(fullContent.Length, needle + MaxDistance);

            Func<int, int> findStartOfWord = suspectIndex =>
                {
                    while (suspectIndex != 0 && !char.IsWhiteSpace(fullContent[suspectIndex]))
                    {
                        suspectIndex--; // find space before word
                    }

                    if (suspectIndex != 0)
                    {
                        suspectIndex++; // skip past space
                    }

                    return suspectIndex;
                };

            start = findStartOfWord(start);

            if (end != fullContent.Length)
            {
                end = findStartOfWord(end);
            }

            return (start > 0 ? "... " : string.Empty) + fullContent.Substring(start, end - start)
                   + (end != fullContent.Length ? " ..." : string.Empty);
        }

        /// <summary>
        ///     Checks if checkAgainst contains any term in dingTerms
        /// </summary>
        /// <param name="checkAgainst">
        ///     The check Against.
        /// </param>
        /// <param name="dingTerms">
        ///     The ding Terms.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasDingTermMatch(this string checkAgainst, IEnumerable<string> dingTerms)
        {
            return dingTerms.Any(term => FirstMatch(checkAgainst, term).Item1 != string.Empty);
        }

        /// <summary>
        ///     Checks if an IMessage is a message which trips our ding terms
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <param name="dingTerms">
        ///     The ding Terms.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsDingMessage(
            this IMessage message, ChannelSettingsModel settings, IEnumerable<string> dingTerms)
        {
            string safeMessage = HttpUtility.HtmlDecode(message.Message);

            if (settings.NotifyIncludesCharacterNames)
            {
                if (message.Poster.Name.HasDingTermMatch(dingTerms))
                {
                    return true;
                }
            }

            if (safeMessage.HasDingTermMatch(dingTerms))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Makes a safe folder path to our channel
        /// </summary>
        /// <param name="character">
        ///     The character.
        /// </param>
        /// <param name="title">
        ///     The title.
        /// </param>
        /// <param name="ID">
        ///     The ID.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string MakeSafeFolderPath(string character, string title, string ID)
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderName;

            if (!title.Equals(ID))
            {
                var safeTitle = Path.GetInvalidPathChars()
                    .Union(new List<char> { ':' })
                    .Aggregate(title, (current, c) => current.Replace(c.ToString(CultureInfo.InvariantCulture), string.Empty));

                if (safeTitle[0].Equals('.'))
                {
                    safeTitle = safeTitle.Remove(0, 1);
                }

                folderName = string.Format("{0} ({1})", safeTitle, ID);
            }
            else
            {
                folderName = ID;
            }

            if (folderName.ContainsOrdinal(@"/", true) || folderName.ContainsOrdinal(@"\", true))
            {
                folderName = folderName.Replace('/', '-');
                folderName = folderName.Replace('\\', '-');
            }

            return Path.Combine(basePath, "slimCat", character, folderName);
        }

        /// <summary>
        ///     The meets chat model lists.
        /// </summary>
        /// <param name="character">
        ///     The character.
        /// </param>
        /// <param name="search">
        ///     The search.
        /// </param>
        /// <param name="cm">
        ///     The cm.
        /// </param>
        /// <param name="channel">
        ///     The channel.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool MeetsChatModelLists(
            this ICharacter character, GenericSearchSettingsModel search, IChatModel cm, GeneralChannelModel channel)
        {
            // notice the toListing, this is an attempt to fix EnumerationChanged errors
            if (cm.Ignored.ToList().Contains(character.Name))
            {
                return search.ShowIgnored;
            }

            if (cm.NotInterested.ToList().Contains(character.Name))
            {
                return search.ShowNotInterested;
            }

            if (cm.Mods.ToList().Contains(character.Name))
            {
                return search.ShowMods;
            }

            if (channel != null)
            {
                if (channel.Moderators.ToList().Contains(character.Name))
                {
                    return search.ShowMods;
                }
            }

            if (cm.Friends.ToList().Contains(character.Name))
            {
                return search.ShowFriends;
            }

            if (cm.Bookmarks.ToList().Contains(character.Name))
            {
                return search.ShowBookmarks;
            }

            return search.MeetsStatusFilter(character);
        }

        /// <summary>
        ///     The meets filters.
        /// </summary>
        /// <param name="character">
        ///     The character.
        /// </param>
        /// <param name="genders">
        ///     The genders.
        /// </param>
        /// <param name="search">
        ///     The search.
        /// </param>
        /// <param name="cm">
        ///     The cm.
        /// </param>
        /// <param name="channel">
        ///     The channel.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool MeetsFilters(
            this ICharacter character, 
            GenderSettingsModel genders, 
            GenericSearchSettingsModel search, 
            IChatModel cm, 
            GeneralChannelModel channel)
        {
            if (!character.NameContains(search.SearchString))
            {
                return false;
            }

            if (!genders.MeetsGenderFilter(character))
            {
                return false;
            }

            return character.MeetsChatModelLists(search, cm, channel);
        }

        /// <summary>
        ///     The meets filters.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="genders">
        ///     The genders.
        /// </param>
        /// <param name="search">
        ///     The search.
        /// </param>
        /// <param name="cm">
        ///     The cm.
        /// </param>
        /// <param name="channel">
        ///     The channel.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool MeetsFilters(
            this IMessage message, 
            GenderSettingsModel genders, 
            GenericSearchSettingsModel search, 
            IChatModel cm, 
            GeneralChannelModel channel)
        {
            if (!message.Poster.NameContains(search.SearchString)
                && !message.Message.ContainsOrdinal(search.SearchString, true))
            {
                return false;
            }

            if (!genders.MeetsGenderFilter(message.Poster))
            {
                return false;
            }

            return message.Poster.MeetsChatModelLists(search, cm, channel);
        }

        /// <summary>
        ///     The name contains.
        /// </summary>
        /// <param name="character">
        ///     The character.
        /// </param>
        /// <param name="searchString">
        ///     The search string.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool NameContains(this ICharacter character, string searchString)
        {
            return character.Name.ToLower().Contains(searchString.ToLower());
        }

        /// <summary>
        ///     The name equals.
        /// </summary>
        /// <param name="character">
        ///     The character.
        /// </param>
        /// <param name="compare">
        ///     The compare.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool NameEquals(this ICharacter character, string compare)
        {
            return character.Name.Equals(compare, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     The relationship to user.
        /// </summary>
        /// <param name="character">
        ///     The character.
        /// </param>
        /// <param name="cm">
        ///     The cm.
        /// </param>
        /// <param name="channel">
        ///     The channel.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string RelationshipToUser(this ICharacter character, IChatModel cm, GeneralChannelModel channel)
        {
            // first, push friends, bookmarks, and moderators to the top of the list
            if (cm.OnlineFriends.Contains(character))
            {
                return "a"; // Really important people!
            }

            if (cm.OnlineBookmarks.Contains(character))
            {
                return "b"; // Important people!
            }

            if (cm.Interested.Contains(character.Name))
            {
                return "c"; // interesting people!
            }

            if (cm.OnlineGlobalMods.Contains(character))
            {
                return "d"; // Useful people!
            }

            if (channel != null && channel.Moderators.Contains(character.Name))
            {
                return "d";
            }

            if (cm.Ignored.Contains(character.Name))
            {
                return "z"; // "I don't want to see this person"
            }

            if (cm.NotInterested.Contains(character.Name))
            {
                return "z"; // I also do not wish to see this person
            }

                // then sort then by status
            switch (character.Status)
            {
                case StatusType.looking:
                    return "e"; // People we want to bone!
                case StatusType.busy:
                    return "f"; // Not the most available, but still possible to play with
                case StatusType.idle:
                case StatusType.away:
                    return "g"; // probably not going to play with, lower on list
                case StatusType.dnd:
                    return "h"; // most likely not going to play with, lowest aside ignored
                default:
                    return "e";
            }
        }

        /// <summary>
        ///     Strips the punctuation in a given string so long as it's at the end.
        ///     Words like it's will not be affected.
        /// </summary>
        /// <param name="fullString">
        ///     The full String.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string StripPunctationAtEnd(this string fullString)
        {
            if (string.IsNullOrWhiteSpace(fullString) || fullString.Length <= 1)
            {
                return fullString;
            }

            var index = fullString.Length - 1;

            while (char.IsPunctuation(fullString[index]) && index != 0)
            {
                index--;
            }

            return index == 0 ? string.Empty : fullString.Substring(0, index);
        }

        #endregion
    }

    /// <summary>
    ///     Keeps the user on the most recent page if things change and if they were on the latest page
    /// </summary>
    public class KeepToCurrentFlowDocument : IDisposable
    {
        #region Fields

        private bool canChangeNow;

        private bool couldChangeBefore;

        private FlowDocumentPageViewer reader;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="KeepToCurrentFlowDocument" /> class.
        /// </summary>
        /// <param name="reader">
        ///     The reader.
        /// </param>
        public KeepToCurrentFlowDocument(FlowDocumentPageViewer reader)
        {
            this.reader = reader;

            this.couldChangeBefore = this.reader.CanGoToNextPage;

            this.reader.Document.DocumentPaginator.PagesChanged += this.KeepToBottom;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     The keep to bottom.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        public void KeepToBottom(object sender, PagesChangedEventArgs e)
        {
            this.canChangeNow = this.reader.CanGoToNextPage;
            if (this.canChangeNow && !this.couldChangeBefore)
            {
                this.reader.NextPage();
            }

            this.couldChangeBefore = this.canChangeNow;
        }

        #endregion

        #region Methods

        private void Dispose(bool managed)
        {
            if (managed)
            {
                this.reader.Document.DocumentPaginator.PagesChanged -= this.KeepToBottom;
                this.reader = null;
            }
        }

        #endregion
    }

    /// <summary>
    /// This handles scroll management for async loading objects.
    /// </summary>
    public class KeepToCurrentScrollViewer
    {
        #region Fields

        private readonly ScrollViewer scroller;

        private double lastValue = 0.0;

        private double lastHeight = 0.0;

        private bool hookedToBottom = true;

        #endregion

        #region Constructors and Destructors

        public KeepToCurrentScrollViewer(DependencyObject toManage)
        {
            toManage.ThrowIfNull("toManage");

            this.scroller = SnapToBottomManager.FindChild(toManage);
            if (this.scroller == null)
            {
                throw new ArgumentException("toManage");
            }

            this.scroller.ScrollChanged += this.OnScrollChanged;
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var difference = Math.Abs(this.scroller.ScrollableHeight - this.lastHeight);
            this.lastHeight = this.scroller.ScrollableHeight;

            if (difference != 0)
            {
                if (this.hookedToBottom)
                {
                    this.scroller.ScrollToBottom();
                }
            }
            else if (Math.Abs(e.VerticalOffset - this.scroller.ScrollableHeight) < 20)
            {
                this.hookedToBottom = true;
            }
            else
            {
                this.hookedToBottom = false;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Stick()
        {
            this.lastValue = this.scroller.ScrollableHeight;
        }

        public void ScrollToStick()
        {
            var change = this.scroller.ScrollableHeight - this.lastValue;
            this.scroller.ScrollToVerticalOffset(this.scroller.VerticalOffset + change);
        }

        #endregion
    }

    /// <summary>
    ///     Caches some int and fetches a new one every so often, displaying how this int has changed
    /// </summary>
    public sealed class CacheCount : IDisposable
    {
        #region Fields

        private readonly int updateResolution;

        private Func<int> getNewCount;

        private bool intialized;

        private int newCount;

        private int oldCount;

        private IList<int> oldCounts;

        private Timer updateTick;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CacheCount" /> class.
        ///     creates a new cached count of something
        /// </summary>
        /// <param name="getNewCount">
        ///     function used to get the new count
        /// </param>
        /// <param name="updateResolution">
        ///     how often, in seconds, it is updated
        /// </param>
        /// <param name="wait">
        ///     The wait.
        /// </param>
        public CacheCount(Func<int> getNewCount, int updateResolution, int wait = 0)
        {
            this.oldCounts = new List<int>();
            this.getNewCount = getNewCount;
            this.updateResolution = updateResolution;

            this.updateTick = new Timer(updateResolution * 1000);
            this.updateTick.Elapsed += (s, e) => this.Update();

            if (wait > 0)
            {
                var waitToStartTick = new Timer(wait);
                waitToStartTick.Elapsed += (s, e) =>
                    {
                        this.updateTick.Start();
                        waitToStartTick.Dispose();
                    };
                waitToStartTick.Start();
            }
            else
            {
                this.updateTick.Start();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     returns the average of the cached values
        /// </summary>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        public double Average()
        {
            return this.oldCounts.Average();
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     The get display string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetDisplayString()
        {
            if (!this.intialized)
            {
                return string.Empty;
            }

            var change = this.newCount - this.oldCount;

            var toReturn = new StringBuilder();
            if (change != 0)
            {
                toReturn.Append("Δ=");
                toReturn.Append(change);
            }

            if (this.oldCounts.Count > 0)
            {
                if (this.Average() != 0)
                {
                    toReturn.Append(" μ=");
                    toReturn.Append(string.Format("{0:0}", this.Average()));
                }

                if (this.StdDev() != 0)
                {
                    toReturn.Append(" σ=");
                    toReturn.Append(string.Format("{0:0.##}", this.StdDev()));
                }

                toReturn.Append(string.Format(" Stability: {0:0.##}%", this.StabilityIndex()));
            }

            return toReturn.ToString().Trim();
        }

        /// <summary>
        ///     returns a measure of how stable the values are
        /// </summary>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        public double StabilityIndex()
        {
            double threshold = this.Average() / 10; // standard deviations above this are considered unstable

            // in this case, an average distance of 20% from our average is considered high
            return Math.Max(Math.Min(Math.Log10(threshold / this.StdDev()) * 100, 100), 0);

            // this scary looking thing just ensures that this value is in between 0 and 100
            // and becomes exponentially closer to 0 as the standard deviation approaches the threshold
        }

        /// <summary>
        ///     returns the adjusted standard deviation for the cached values
        /// </summary>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        public double StdDev()
        {
            var squares = this.oldCounts.Select(x => Math.Pow(x - this.Average(), 2)).ToList();

            // this is the squared distance from average
            return Math.Sqrt(squares.Sum() / (squares.Count() > 1 ? squares.Count() - 1 : squares.Count()));

            // calculates population std dev from our sample
        }

        /// <summary>
        ///     The update.
        /// </summary>
        public void Update()
        {
            if (this.intialized)
            {
                this.oldCount = this.newCount;

                // 60/updateres*30 returns how many update resolutions fit in 30 minutes
                if (this.oldCounts.Count > ((60 / this.updateResolution) * 30))
                {
                    this.oldCounts.RemoveAt(0);
                }

                this.oldCounts.Add(this.oldCount);

                this.newCount = this.getNewCount();
            }
            else
            {
                this.oldCount = this.newCount = this.getNewCount();

                if (!(this.oldCount == 0 || this.newCount == 0))
                {
                    this.intialized = true;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The dispose.
        /// </summary>
        /// <param name="isManaged">
        ///     The is managed.
        /// </param>
        private void Dispose(bool isManaged)
        {
            if (!isManaged)
            {
                return;
            }

            this.updateTick.Stop();
            this.updateTick.Dispose();
            this.updateTick = null;

            this.getNewCount = null;
            this.oldCounts.Clear();
            this.oldCounts = null;
        }

        #endregion
    }
}