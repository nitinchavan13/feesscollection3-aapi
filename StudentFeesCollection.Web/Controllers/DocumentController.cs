using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace StudentFeesCollection.Web.Controllers
{
    public class DocumentController : ApiController
    {
        //public IHttpActionResult GeneratePdf()
        //{
        //    try
        //    {
        //        //var Renderer = new IronPdf.ChromePdfRenderer();
        //        //var PDF = Renderer.RenderHtmlAsPdf("<h1>Hello IronPdf</h1>");
        //        //var path = PDF.SaveAs("pixel-perfect.pdf");
        //        //HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
        //        //httpResponseMessage.Content = new StreamContent(path.Stream);
        //        //httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
        //        //httpResponseMessage.Content.Headers.ContentDisposition.FileName = "first.pdf";
        //        //httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        //        //return httpResponseMessage;
        //        var Renderer = new ChromePdfRenderer();
        //        var myPdf = Renderer.RenderUrlAsPdf("https://www.nuget.org/packages/IronPdf");
        //        // Watermarks all pages with red "SAMPLE" text at a custom location.
        //        // Also adding a link to the watermark on-click
        //        myPdf.WatermarkAllPages("<h2 style='color:red'>SAMPLE</h2>", IronPdf.Editing.WaterMarkLocation.MiddleCenter, 50, -45, "https://www.nuget.org/packages/IronPdf");
        //        myPdf.SaveAs(@"C:\Path\To\Watermarked.pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        public HttpResponseMessage Display(string docid)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            var renderer = new IronPdf.ChromePdfRenderer();
            var pdf = renderer.RenderHtmlAsPdf("<h1>hello ironpdf</h1>");
            byte[] buffer = new byte[0];
            //generate pdf document
            var path = pdf.SaveAs("pixel-perfect.pdf");
            //get buffer
            buffer = path.Stream.ToArray();
            //content length for use in header
            var contentLength = buffer.Length;
            //200
            //successful
            var statuscode = HttpStatusCode.OK;
            response = Request.CreateResponse(statuscode);
            response.Content = new StreamContent(new MemoryStream(buffer));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            response.Content.Headers.ContentLength = contentLength;
            ContentDispositionHeaderValue contentDisposition = null;
            if (ContentDispositionHeaderValue.TryParse("inline; filename=test.pdf", out contentDisposition))
            {
                response.Content.Headers.ContentDisposition = contentDisposition;
            }

            return response;
        }
    }
}
