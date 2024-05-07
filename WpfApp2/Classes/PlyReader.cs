using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using GameEngine.Classes;
using System.Security.Cryptography;
using System.ComponentModel;

namespace GameEngine
{
  



    public class Header 
    {
        //public ElementPropertyAttribute[] Elements = new ElementPropertyAttribute[10];
        public Dictionary<string, Object> Elements;
        public Dictionary<string, Object> Properties;
        public string fileType;
        public int vertexCount;
        public int faceCount;
        public bool hasNormals = false;

        public Header()
        {
            Elements = new Dictionary<string, Object>();

        }
    }
    public class PlyReader// : Attribute
    {
        StreamReader rdr;
        BinaryReader binReader;
        Header header;
        FileStream file;
        int pos;
        List<byte> buffer = new List<byte>();
        

        PlyReader() { }
        //public PlyReader(byte[] data)
        //{

        //}

        public PlyReader(FileStream f)
        {
            header = new Header();
            rdr = new StreamReader(f);
            file = f;
        }

        public Header ReadHeader()
        {
            string token_string; // pointer to actual token text, ready for parsing
            token_string = rdr.ReadLine();
            pos = token_string.Length +1;
            string[] token;
            token = token_string.Split(new Char[] { ' ' });
            if (token.Length > 0)
            {
                if (token[0] != "ply")
                {
                    return null;
                }
                do
                {
                    token_string = rdr.ReadLine();
                    pos += token_string.Length+1;
                    token = token_string.Split(new Char[] { ' ' });
                    if (token[0] == "format")
                    {
                        if (token[1] == "ascii")
                        {
                            header.fileType = "ascii";
                        }
                        else if (token[1].Substring(0, 6) == "binary")
                        {
                            header.fileType = "binary";
                            binReader = new BinaryReader(file, Encoding.BigEndianUnicode);
                        }
                    }
                    else if (token[0] == "element")
                    {
                        if (token[1] == "vertex")
                        {
                            header.Elements["vertex"] = new List<Vertex>();
                            header.vertexCount = Convert.ToInt32(token[2]);
                        }
                        else if (token[1] == "face")
                        {
                            header.Elements["face"] = new List<Face>();
                            header.faceCount = Convert.ToInt32(token[2]);
                        }
                    }
                    else if (token[0] == "property")
                    {
                        if (token[1] == "float")
                        {
                            if (token[2].StartsWith("n"))
                            {
                                header.hasNormals = true;
                            }
                        }
                        else if (token[1] == "uchar")
                        {

                        }
                    }
                } while (token_string != "end_header");
               // pos += token_string.Length;
                if (header.fileType == "binary")
                {
                    //pos += token_string.Length;// +3;
                    binReader.BaseStream.Seek(pos, SeekOrigin.Begin);
                }

            }
            else
            {
                return null;
            }

            //throw new NotImplementedException();
            return header;
        }

        //public string ReadString()
        //{
        //    string str = "";
        //    short size = binReader.ReadInt16();
        //    byte[] estring = new byte[size];

        //    for (int x = pos, i = 0; x < pos + size; x++, i++)
        //    {
        //        estring[i] = this.buffer[x];
        //    }

        //    foreach (byte x in estring)
        //    {
        //        str += (char)x;
        //    }

        //    pos += size;
        //    return str;
        //}

        //public float FromFloatSafe(Byte[] bytes)
        //{            
        //    uint fb = BitConverter.ToUInt32(bytes, 0); //Convert.ToUInt32(f);

        //    int sign = (int)((fb >> 31) & 1);
        //    int exponent = (int)((fb >> 23) & 0xFF);
        //    int mantissa = (int)(fb & 0x7FFFFF);

        //    float fMantissa;
        //    float fSign = sign == 0 ? 1.0f : -1.0f;

        //    if (exponent != 0)
        //    {
        //        exponent -= 127;
        //        fMantissa = 1.0f + (mantissa / (float)0x800000);
        //    }
        //    else
        //    {
        //        if (mantissa != 0)
        //        {
        //            // denormal
        //            exponent -= 126;
        //            fMantissa = 1.0f / (float)0x800000;
        //        }
        //        else
        //        {
        //            // +0 and -0 cases
        //            fMantissa = 0;
        //        }
        //    }

        //    float fExponent = (float)Math.Pow(2.0, exponent);
        //    float ret = fSign * fMantissa * fExponent;
        //    return ret;
        //}

        //float IEEE_754_to_float(byte[] raw)
        //{
        //    int sign = (raw[0] >> 7) != 0 ? -1 : 1;

        //    float exponent = (raw[0] << 1) + (raw[1] >> 7) - 127;// 126;
        //    int b = (raw[1] & 0x7F);
        //    int c = b << 16;

        //    int mantissa_bits = ((raw[1] & 0x7F) << 16) + (raw[2] << 8) + raw[3];
        //    float fMantissa = 1.0f + (mantissa_bits / (float)0x800000);
           
        //    float fExponent = (float)Math.Pow(2.0, exponent);
        //    float result = sign * fMantissa * fExponent;
           
        //    return result;
        //}

        float IEEE_754_to_float(byte[] raw) {
            int sign = (raw[0] >> 7) !=0 ? -1 : 1;
            int iExponent = (raw[0] << 1) + (raw[1] >> 7);
            int exponent = iExponent - 127;            
            int b = (raw[1] & 0x7F);
            int c = b << 16;

            int mantissa_bits = ((raw[1] & 0x7F) << 16) + (raw[2] << 8) + raw[3];
            int numOfZeros = 0;
            int numOfBitsInMantissa = 0;
            int a = 0;
            for (int i = 23; i >= 1; i--)
            {
                a = (mantissa_bits >> (23 - i)) & 1;
                if (a == 0)
                    numOfZeros++;
                else
                    break;
            }
            int normalizedMantissa = mantissa_bits >> numOfZeros;
            numOfBitsInMantissa = 23 - numOfZeros;
            
            float fMantissa = 0;
          
            float x = 0;
           
            for (int ii = 1; ii <= 23; ++ii)
            {
                x = (mantissa_bits >> (23 - ii)) & 1;
                x *= 2;
                if (x > 0)
                {         
                    fMantissa += (float)Math.Pow(x, -ii);
                }
            }
            fMantissa += 1.0f;
           
            float fExponent = (float)Math.Pow(2.0, exponent);
            float result = sign * fMantissa * fExponent;
            
            return result;
        }


    public Vertex ReadVertices(int index)
        {
           // List<Vertex> vertices = (List<Vertex>)header.Elements["vertex"];
            string token_string = ""; // pointer to actual token text, ready for parsing
            string[] token = new string[0];
            
            byte[] fakeBytes;
            byte[] bytes = new byte[4];
            if (header.fileType == "binary")
            {
                //binReader.BaseStream.Seek(pos, SeekOrigin.Begin);

             //   token = binReader.ReadBytes(4);
            }
            if (header.fileType == "ascii")
            {
                token_string = rdr.ReadLine();
                token = token_string.Split(new Char[] { ' ' });
            }
            else
            {

            }


            //for (int i = 0; i < header.vertexCount; i++)
            //{
            float x =0;
            float y =0;
            float z =0;            
            float nx = 0;
            float ny = 0;
            float nz = 0;
            float s = 0, t = 0;

            if (header.fileType == "ascii")
            {
                x = Convert.ToSingle(token[0]);//Convert.ToUInt16(token[0]);
                y = Convert.ToSingle(token[1]);
                z = Convert.ToSingle(token[2]);
            }
            else
            {
                //float f = binReader.ReadSingle();
                //bytes = binReader.ReadBytes(4);
                //bytes[0] = 66;
                //bytes[1] = 170;
                //bytes[2] = 64;
                //bytes[3] = 0;

                // Array.Reverse(bytes);                
                x = binReader.ReadSingle()*200;//IEEE_754_to_float(bytes);//BitConverter.ToSingle(bytes, 0); //Convert.ToUInt16(binReader.ReadBytes(4));// binReader.ReadUInt16();               
                //bytes = binReader.ReadBytes(4);              
                //Array.Reverse(bytes);
                y = binReader.ReadSingle() * 200;//IEEE_754_to_float(bytes); //BitConverter.ToSingle(bytes, 0);//Convert.ToUInt16(binReader.ReadBytes(4));// binReader.ReadUInt16();

                //bytes = binReader.ReadBytes(4);              
                //Array.Reverse(bytes);
                z = binReader.ReadSingle() * 200;//IEEE_754_to_float(bytes); //BitConverter.ToSingle(bytes, 0);//Convert.ToUInt16(binReader.ReadBytes(4));// binReader.ReadUInt16();

                if (header.hasNormals)
                { 
                    nx = binReader.ReadSingle();                
                    ny = binReader.ReadSingle();              
                    nz = binReader.ReadSingle();
                }

                //bytes = binReader.ReadBytes(4);               
                //Array.Reverse(bytes);
                s = binReader.ReadSingle(); // BitConverter.ToSingle(bytes, 0); 

                //bytes = binReader.ReadBytes(4);               
                //Array.Reverse(bytes);
                t = binReader.ReadSingle(); //BitConverter.ToSingle(bytes, 0); 
            }
            Vertex vertex = new Vertex(x, y, z);
            return vertex;
        }

        public Face ReadFaces(int index)
        {            
            string token_string; // pointer to actual token text, ready for parsing
            string[] token = new string[0];
            if (header.fileType == "ascii")
            { 
                token_string = rdr.ReadLine();
                token = token_string.Split(new Char[] { ' ' });
            }                       
           
            Face face = new Face();

            byte[] bytes = new byte[4];

            if (header.fileType == "ascii")
            { 
                if (token!=null)
                    face.VertexCount = Convert.ToInt32(token[0]);
            }
            else if (header.fileType == "binary")
            {                
                face.VertexCount = (int)binReader.ReadByte();// binReader.ReadChar();//binReader.ReadInt32();
            }
        
            face.VertexIndices = new uint[face.VertexCount];
            for (int i = 0; i < face.VertexCount; i++)
            {              
                if (header.fileType == "ascii")
                { 
                    face.VertexIndices[i] = Convert.ToUInt32(token[i+1]);
                }
                else if (header.fileType == "binary")
                {              
                    face.VertexIndices[i] = (uint)binReader.ReadInt32();//BitConverter.ToUInt32(bytes, 0); //Convert.ToUInt32(token[i + 1]);
                }
            }
            return face;
        }
    }
}
