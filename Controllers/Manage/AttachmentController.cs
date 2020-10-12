using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApi.Controllers.Manage
{
    [RoutePrefix("manage/")]

    public class AttachmentController : ApiController
    {
        private readonly DataBase.DB db = new DataBase.DB();
        private const string TOKEN = "ZFDYES";
        /// <summary>
        /// 单个文件上传
        /// 返回附件Id
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> UploadAttachment()
        {
            Guid userId = Helper.EncryptionHelper.GetUserId(HttpContext.Current.Request.Headers[TOKEN]);


            var files = HttpContext.Current.Request.Files;
            if (files != null && files.Count > 0)
            {
                    var now = DateTime.Now;
                    string attachmentType = HttpContext.Current.Request.Headers["AttachmentType"];
                    var fileExtension = Path.GetExtension(files[0].FileName);    //扩展名

                    var attachment = new DataBase.Attachments 
                    { 
                        Id = Guid.NewGuid(),
                        UpTime = now,
                        UpAccount = userId,
                        Belong = Guid.Empty,//保存成功的时候修改
                        Status = Models.Config.Status.forbidden,
                        AttachmentType = string.IsNullOrWhiteSpace(attachmentType) ? "-1" : attachmentType,
                        FileType = files[0].ContentType,
                        FileSize = files[0].ContentLength,
                        FileName = files[0].FileName,
                        FileExt = fileExtension
                    };
                db.Entry(attachment).State = System.Data.Entity.EntityState.Added;
                try
                {
                    await db.SaveChangesAsync();
                    string UploadRoot = ConfigurationManager.AppSettings["FilePath"].ToString();    //根路径
                    var saveFilePath = UploadRoot + "/" + now.ToString("yyyyMMdd") + "/" + userId.ToString("N");

                    var fileSize = files[0].ContentLength;
                    if (fileSize > 10485760)
                    {
                        return Json(new { status = "fail", msg = "请上传10M以内的文件!" });
                    }

                    //保存附件
                    if (!Directory.Exists(saveFilePath)) Directory.CreateDirectory(saveFilePath);
                    //saveFilePath 为文件的物理路径 --- /根路径/时间/上传人ID/文件名.扩展名
                    files[0].SaveAs(saveFilePath + "/" + files[0].FileName + fileExtension);

                    return Json(new { status = "success", msg = "上传成功!", content = attachment.Id });
                }
                catch(Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                    return Json(new { status = "fail", msg = "服务器内部错误!" });
                }
                    
            }
            else
            {
                return Json(new { status = "fail", msg = "上传文件为空!" });
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public async void Download(string Code)
        {
            Guid code = Helper.TypeHelper.ToGuid(Code);

            var attachment = await db.Attachments.FindAsync(code);
            if (attachment == null) return;

            string filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            if (Directory.Exists(filepath) == false) Directory.CreateDirectory(filepath);         //如果不存在就创建file文件夹
            filepath = filepath + "/" + attachment.UpTime.ToString("yyyyMMdd") + "/" + attachment.UpAccount.ToString("N") + "/" + attachment.FileName + attachment.FileExt;
            //根路径+年月日+上传人Id+附件名.png
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filepath);
            if (fileInfo.Exists == true)
            {
                try
                {
                    const long ChunkSize = 204800;
                    byte[] buffer = new byte[ChunkSize];

                    HttpContext.Current.Response.Clear();
                    System.IO.FileStream iStream = System.IO.File.OpenRead(filepath);
                    long dataLengthToRead = iStream.Length;
                    HttpContext.Current.Response.ContentType = attachment.FileType;
                    HttpContext.Current.Response.AddHeader("Content-Length", dataLengthToRead.ToString());
                    HttpContext.Current.Response.AddHeader("Content-Disposition",
                        "attachment; filename=" + attachment.FileName);
                    while (dataLengthToRead > 0 && HttpContext.Current.Response.IsClientConnected)
                    {
                        int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(ChunkSize)); //读取的大小  
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, lengthRead);
                        HttpContext.Current.Response.Flush();
                        dataLengthToRead = dataLengthToRead - lengthRead;
                    }
                    HttpContext.Current.Response.Close();
                    iStream.Close();
                }
                catch (Exception ex)
                {
                    Helper.LogHelper.WriteErrorLog(ex);
                }
            }
        }




        //[AllowAnonymous]
        //public IHttpActionResult ChangeAttachmentStatus(Models.Request.Co.Attachment.UpdateAttachment m)
        //{
        //    int AttachmentStatus = m.AttachmentStatus;
        //    Guid AttachmentId = Helper.TypeHelper.ToGuid(m.AttachmentId);

        //    if (AttachmentId == Guid.Empty)
        //    {
        //        return Json(new { status = "fail", msg = "请求删除的附件ID为空!", });
        //    }
        //    try
        //    {
        //        var targetAttachment = Factory.DataBase.Attachment.Find(AttachmentId);

        //        if (targetAttachment == null)
        //        {
        //            return Json(new { status = "fail", msg = "未找到请求删除的附件!", });
        //        }

        //        targetAttachment.Status = AttachmentStatus;
        //        Factory.DataBase.Attachment.Update(targetAttachment);

        //        return Json(new { status = "success", msg = "删除成功!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.LogHelper.WriteErrorLog(ex);
        //        return Json(new { status = "fail", msg = "删除失败!", });
        //    }
        //}
    }
}
