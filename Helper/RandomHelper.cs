using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class RandomHelper
    {
        #region 获取随机字符串
        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="codeLength">字符串长度</param>
        /// <returns></returns>
        public static string GetCodeStr(int codeLength)
        {
            string charCollection = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] charCollections = charCollection.Split(',');
            string[] str = new string[codeLength];
            string serverCode = "";
            //生成随机生成器 
            Random random = new Random();
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = charCollections[random.Next(charCollections.Length)];
            }

            foreach (string s in str)
            {
                serverCode += s;
            }
            return serverCode;
        }
        #endregion
    }
}
