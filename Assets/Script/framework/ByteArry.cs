using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ByteArry
{
      //默认大小
      const int DEFAULT_SIZE = 1024;
      //初始大小
      int initSize = 0;
      //缓冲区
      public byte[] bytes;
      //读写位置
      public int readIdx = 0;
      public int writeIdx = 0;
      //容量
      private int capacity = 0;
      //剩余空间
      public int remain { get { return capacity - writeIdx; } }
      /// <summary>
      /// 数据长度
      /// </summary>
      public int length { get { return writeIdx - readIdx; } }
      //构造函数
      public ByteArry(byte[] defaultBytes)
      {
            bytes = defaultBytes;
            capacity = defaultBytes.Length;
            initSize = defaultBytes.Length;
            readIdx = 0;
            writeIdx = defaultBytes.Length;
      }
      //构造函数
      public ByteArry(int size = DEFAULT_SIZE)
      {
            bytes = new byte[size];
            capacity = size;
            initSize = size;
            readIdx = 0;
            writeIdx = 0;
      }
      public void ReSize(int size)
      {
            if (size < length || size < initSize)
                  return;
            int n = 1;
            while (n < size)
            { n *= 2; }
            capacity = n;
            byte[] newBytes = new byte[capacity];
            Array.Copy(bytes, readIdx, newBytes, 0, writeIdx - readIdx);
            bytes = newBytes;
            writeIdx = length;
            readIdx = 0;
      }
      //(当数据很小的时候)检测并移动数据
      public void CheckAndMove()
      {
            if(length<8)
            { MoveBytes(); }
      }
      //移动数据
      public void MoveBytes()
      {
            if (length > 0)
            {
                  Array.Copy(bytes, readIdx, bytes, 0, length);
            }
            writeIdx = length;
            readIdx = 0;
            
      }
      /// <summary>
      /// 把bs的数据写入到数据缓冲区
      /// </summary>
      /// <param name="bs"></param>
      /// <param name="offset">从bs的offset下标开始写入</                                                                                                                         param>
      /// <param name="count">需要写入的长度</param>
      /// <returns></returns>
      public int Write(byte[] bs,int offset,int count)
      {
            if(remain<count)
            {
                  ReSize(length + count);
            }
            Array.Copy(bs, offset, bytes, writeIdx, count);
            writeIdx += count;
            return count;
      }
      /// <summary>
      /// 把数据缓冲区的数据读取到bs
      /// </summary>
      /// <param name="bs"></param>
      /// <param name="offset">从bs的offset下标开始存</param>
      /// <param name="count">缓冲区前count个数据</param>
      /// <returns></returns>
      public int Read(byte[] bs, int offset, int count)
      {
            count = Math.Min(count, length);
            Array.Copy(bytes, readIdx, bs, offset, count); 
            readIdx += count;
            CheckAndMove();
            return count;
      }
      /// <summary>
      /// 把数据缓冲区的数据读取到bs
      /// </summary>
      /// <param name="bs"></param>
      /// <param name="offset">从数据缓冲区的指定下标开始</param>
      /// <param name="count">数据长度</param>
      /// <returns></returns>
      public int ReadC(byte[]bs,int offset,int count)
      {
            count = Math.Min(count, length);
            Array.Copy(bytes, offset, bs, 0, count);
            readIdx += count;
            CheckAndMove();
            return count;
      }
      /// <summary>
      /// 读取Int16
      /// </summary>
      public Int16 ReadInt16()
      {
            if(length<2)
            { return 0; }
            Int16 ret = (Int16)((bytes[1] << 8) | bytes[0]);
            readIdx += 2;
            CheckAndMove();
            return ret;
      }
      /// <summary>
      /// 读取Int32 7
      /// </summary>
      public Int32 ReadInt32()
      {
            if (length<4)
            { return 0; }
            Int32 ret = (Int32)((bytes[3] << 24) |
                                          bytes[2] << 16 |
                                          bytes[1] << 8 |
                                          bytes[0]);
            readIdx += 4;
            CheckAndMove();
            return ret;
      }
}
