using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AceNetFrameWork.ace
{
   public class UserTokenPool
    {
        private Stack<UserToken> pool;

        public UserTokenPool(int size){
            pool = new Stack<UserToken>();
        }

        public UserToken pop() {
            UserToken token=            pool.Pop();
            return token;
        }

        public void push(UserToken item) {
            if(item!=null)
            pool.Push(item);
        }

        public int getSize() {
            return pool.Count;
        }
    }
}
