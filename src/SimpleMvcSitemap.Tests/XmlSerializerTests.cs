﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace SimpleMvcSitemap.Tests
{
    public class XmlSerializerTests : TestBase
    {
        private IXmlSerializer _serializer;

        protected override void FinalizeSetUp()
        {
            _serializer = new XmlSerializer();
        }

        [Test]
        public void Serialize_SitemapModel()
        {
            SitemapModel sitemap = new SitemapModel(new List<SitemapNode> { new SitemapNode("abc"), new SitemapNode("def") });

            string result = Serialize(sitemap);

            result.Should().BeXmlEquivalent("Samples/sitemap.xml");
        }

        [Test]
        public void Serialize_SitemapIndexModel()
        {
            SitemapIndexModel sitemapIndex = new SitemapIndexModel(new List<SitemapIndexNode>
            {
                new SitemapIndexNode { Url = "abc" },
                new SitemapIndexNode { Url = "def" }
            });

            string result = Serialize(sitemapIndex);

            result.Should().BeXmlEquivalent("Samples/sitemap-index.xml");
        }

        [Test]
        public void Serialize_SitemapNode_RequiredTegs()
        {
            SitemapNode sitemapNode = new SitemapNode("abc");

            string result = SerializeSitemap(sitemapNode);

            result.Should().BeXmlEquivalent("Samples/sitemap-node-required.xml");
        }

        [Test]
        public void Serialize_SitemapNode_AllTags()
        {
            SitemapNode sitemapNode = new SitemapNode("abc")
            {
                LastModificationDate = new DateTime(2013, 12, 11, 16, 05, 00, DateTimeKind.Utc),
                ChangeFrequency = ChangeFrequency.Weekly,
                Priority = 0.8M
            };

            string result = SerializeSitemap(sitemapNode);

            result.Should().BeXmlEquivalent("Samples/sitemap-node-all.xml");
        }

        [Test]
        public void Serialize_SitemapIndexNode_RequiredTags()
        {
            SitemapIndexNode sitemapIndexNode = new SitemapIndexNode("abc");

            string result = Serialize(sitemapIndexNode);

            result.Should().BeXmlEquivalent("Samples/sitemap-index-node-required.xml");
        }

        [Test]
        public void Serialize_SitemapIndexNode_AllTags()
        {
            SitemapIndexNode sitemapIndexNode = new SitemapIndexNode
            {
                Url = "abc",
                LastModificationDate = new DateTime(2013, 12, 11, 16, 05, 00, DateTimeKind.Utc)
            };

            string result = Serialize(sitemapIndexNode);

            result.Should().BeXmlEquivalent("Samples/sitemap-index-node-all.xml");
        }

        [Test]
        public void Serialize_SitemapNode_ImageRequiredTags()
        {
            SitemapNode sitemapNode = new SitemapNode("abc")
            {
                Images = new List<SitemapImage> { new SitemapImage("image1"), new SitemapImage("image2") }
            };

            string result = SerializeSitemap(sitemapNode);

            result.Should().BeXmlEquivalent("Samples/sitemap-node-image-required.xml");
        }

        [Test]
        public void Serialize_SitemapNode_ImageAllTags()
        {
            SitemapNode sitemapNode = new SitemapNode("abc")
            {
                Images = new List<SitemapImage>
                {
                    new SitemapImage("http://example.com/image.jpg")
                    {
                        Caption = "Photo caption",
                        Location = "Limerick, Ireland",
                        License = "http://choosealicense.com/licenses/unlicense/",
                        Title = "Photo Title"
                    }
                }
            };

            string result = SerializeSitemap(sitemapNode);

            result.Should().BeXmlEquivalent("Samples/sitemap-node-image-all.xml");
        }

        [Test]
        public void Serialize_SitemapNode_VideoRequiredTags()
        {
            SitemapNode sitemapNode = new SitemapNode("http://www.example.com/videos/some_video_landing_page.html")
            {
                Video = new SitemapVideo("Grilling steaks for summer", "Alkis shows you how to get perfectly done steaks every time",
                                         "http://www.example.com/thumbs/123.jpg", "http://www.example.com/video123.flv")
            };

            string result = SerializeSitemap(sitemapNode);

            result.Should().BeXmlEquivalent("Samples/sitemap-node-video-required.xml");
        }

        [Test]
        public void Serialize_SitemapNode_VideoAllTags()
        {
            SitemapNode sitemapNode = new SitemapNode("http://www.example.com/videos/some_video_landing_page.html")
            {
                Video = new SitemapVideo("Grilling steaks for summer", "Alkis shows you how to get perfectly done steaks every time",
                                         "http://www.example.com/thumbs/123.jpg", "http://www.example.com/video123.flv")
                {
                    PlayerUrl = new VideoPlayerUrl("http://www.example.com/videoplayer.swf?video=123")
                    {
                        AllowEmbed = YesNo.Yes,
                        Autoplay = "ap=1"
                    },
                    Duration = 600,
                    ExpirationDate = new DateTime(2014, 12, 16, 16, 56, 0, DateTimeKind.Utc),
                    Rating = 4.2M,
                    ViewCount = 12345,
                    PublicationDate = new DateTime(2014, 12, 16, 17, 51, 0, DateTimeKind.Utc),
                    FamilyFriendly = YesNo.No,
                    Tags = new[] { "steak", "summer", "outdoor" },
                    Category = "Grilling",
                    Restriction = new VideoRestriction("IE GB US CA", VideoRestrictionRelationship.Allow),
                    Gallery = new VideoGallery("http://cooking.example.com")
                    {
                        Title = "Cooking Videos"
                    },
                    Prices = new List<VideoPrice>
                    {
                        new VideoPrice("EUR",1.99M),
                        new VideoPrice("TRY",5.99M){Type = VideoPurchaseOption.Rent},
                        new VideoPrice("USD",2.99M){Resolution = VideoPurchaseResolution.Hd}
                    },
                    RequiresSubscription = YesNo.No,
                    Uploader = new VideoUploader("GrillyMcGrillerson")
                    {
                        Info = "http://www.example.com/users/grillymcgrillerson"
                    },
                    Platform = "web mobile",
                    Live = YesNo.Yes
                }
            };

            string result = SerializeSitemap(sitemapNode);

            result.Should().BeXmlEquivalent("Samples/sitemap-node-video-all.xml");
        }

        [Test]
        public void Serialize_SitemapNode_NewsReqiredTags()
        {
            SitemapNode sitemapNode = new SitemapNode("http://www.example.org/business/article55.html")
            {
                News = new SitemapNews(new NewsPublication("The Example Times", "en"), new DateTime(2014, 11, 5, 0, 0, 0, DateTimeKind.Utc), "Companies A, B in Merger Talks")
            };

            string result = SerializeSitemap(sitemapNode);

            result.Should().BeXmlEquivalent("Samples/sitemap-node-news-required.xml");
        }

        [Test]
        public void Serialize_SitemapNode_NewsAllTags()
        {
            SitemapNode sitemapNode = new SitemapNode("http://www.example.org/business/article55.html")
            {
                News = new SitemapNews(new NewsPublication("The Example Times", "en"), new DateTime(2014, 11, 5, 0, 0, 0, DateTimeKind.Utc), "Companies A, B in Merger Talks")
                {
                    Access = NewsAccess.Subscription,
                    Genres = "PressRelease, Blog",
                    Keywords = "business, merger, acquisition, A, B",
                    StockTickers = "NASDAQ:A, NASDAQ:B"
                }
            };

            string result = SerializeSitemap(sitemapNode);

            result.Should().BeXmlEquivalent("Samples/sitemap-node-news-all.xml");
        }

        [Test]
        public void Serialize_SitemapNode_Mobile()
        {
            SitemapNode sitemapNode = new SitemapNode("http://mobile.example.com/article100.html") { Mobile = new SitemapMobile() };

            string result = SerializeSitemap(sitemapNode);

            result.Should().BeXmlEquivalent("Samples/sitemap-node-mobile.xml");
        }

        [Test]
        public void Serialize_SitemapModel_AlternateLinks()
        {
            SitemapModel sitemap = new SitemapModel(new List<SitemapNode> { new SitemapNode("abc", new List<SitemapUrlLink>
            {
                new SitemapUrlLink("cba", "de")
            }), new SitemapNode("def", new List<SitemapUrlLink>
            {
                new SitemapUrlLink("fed", "de")
            }) });

            string result = Serialize(sitemap);

            result.Should().BeXmlEquivalent("Samples/sitemap-alternate-links.xml");
        }

        private string SerializeSitemap(SitemapNode sitemapNode)
        {
            return Serialize(new SitemapModel(new[] { sitemapNode }));
        }

        private string Serialize<T>(T data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                _serializer.SerializeToStream(data,stream);
                stream.Seek(0, SeekOrigin.Begin);
                return new StreamReader(stream).ReadToEnd();
            }
        }

    }
}