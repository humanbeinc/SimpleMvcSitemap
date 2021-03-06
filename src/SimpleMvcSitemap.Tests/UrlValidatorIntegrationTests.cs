﻿using System;
using System.Collections.Generic;
using System.Web;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace SimpleMvcSitemap.Tests
{
    public class UrlValidatorIntegrationTests : TestBase
    {
        private IUrlValidator _urlValidator;

        protected override void FinalizeSetUp()
        {
            Mock<IBaseUrlProvider> baseUrlProvider = MockFor<IBaseUrlProvider>();
            _urlValidator = new UrlValidator(new ReflectionHelper(), baseUrlProvider.Object);

            baseUrlProvider.Setup(item => item.GetBaseUrl(It.IsAny<HttpContextBase>())).Returns("http://example.org");
        }

        [Test]
        public void ValidateUrls_SitemapNode()
        {
            SitemapNode siteMapNode = new SitemapNode("/categories");

            _urlValidator.ValidateUrls(null, siteMapNode);

            siteMapNode.Url.Should().Be("http://example.org/categories");
        }

        [Test]
        public void ValidateUrls_SitemapIndexNode()
        {
            SitemapIndexNode sitemapIndexNode = new SitemapIndexNode("/product-sitemap");

            _urlValidator.ValidateUrls(null, sitemapIndexNode);

            sitemapIndexNode.Url.Should().Be("http://example.org/product-sitemap");
        }

        [Test]
        public void ValidateUrls_SitemapNodeWithImages()
        {
            SitemapNode sitemapNode = new SitemapNode("abc")
            {
                Images = new List<SitemapImage> 
                { 
                    new SitemapImage("/image.jpg")
                    {
                        License = "/licenses/unlicense/",
                    }
                }
            };

            _urlValidator.ValidateUrls(null, sitemapNode);

            var sitemapImage = sitemapNode.Images[0];

            sitemapImage.Url.Should().Be("http://example.org/image.jpg");
            sitemapImage.License.Should().Be("http://example.org/licenses/unlicense/");
        }

        [Test]
        public void ValidateUrls_SitemapNodeWithVideo()
        {
            SitemapNode sitemapNode = new SitemapNode("/some_video_landing_page.html")
            {
                Video = new SitemapVideo
                {
                    ContentUrl = "/video123.flv",
                    ThumbnailUrl = "/thumbs/123.jpg",
                    PlayerUrl = new VideoPlayerUrl
                    {
                        Url = "/videoplayer.swf?video=123",
                    },
                    Gallery = new VideoGallery
                    {
                        Url = "/gallery-1",
                    },
                    Uploader = new VideoUploader
                    {
                        Info = "/users/grillymcgrillerson"
                    }
                }
            };

            _urlValidator.ValidateUrls(null, sitemapNode);

            sitemapNode.Video.ContentUrl.Should().Be("http://example.org/video123.flv");
            sitemapNode.Video.ThumbnailUrl.Should().Be("http://example.org/thumbs/123.jpg");
            sitemapNode.Video.PlayerUrl.Url.Should().Be("http://example.org/videoplayer.swf?video=123");
            sitemapNode.Video.Gallery.Url.Should().Be("http://example.org/gallery-1");
            sitemapNode.Video.Uploader.Info.Should().Be("http://example.org/users/grillymcgrillerson");
        }

    }
}