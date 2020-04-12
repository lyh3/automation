using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;

namespace McAfeeLabs.Engineering.Automation.Base
{
    static public class DeepSerialization
    {
        [ThreadStatic]
        private static BinaryFormatter _formatter = null;
        [ThreadStatic]
        private static SoapFormatter _soapFormater = null;

        public static T DeepClone<T>(this T sourceObj)
        {
            if (_formatter == null) 
                _formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                _formatter.Serialize(stream, sourceObj);
                stream.Position = 0;
                return (T)_formatter.Deserialize(stream);
            }
        }

        /// <summary>
        ///  Binary Serializes an object
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>byte array</returns>
        public static byte[] BinarySerialize(this Object obj)
        {
            const int ChunkSize = 100 * 1024 * 1024;
            byte[] serializedObject;

            if (_formatter == null)
                _formatter = new BinaryFormatter();   
         
            using (var stream = new MemoryStream())
            {
                var br = new BinaryReader(stream);
                _formatter.Serialize(stream, obj);
                stream.Seek(0, 0); 

                byte[] outbyte = new byte[ChunkSize];
                int retval;
                retval = br.Read(outbyte, 0, ChunkSize);
                while (retval > 0)
                {
                    stream.Write(outbyte, 0, retval);
                    stream.Flush();
                    retval = br.Read(outbyte, 0, ChunkSize);
                }

                serializedObject = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(serializedObject, 0, (int)stream.Length);
            }

            return serializedObject;
        }

        /// <summary>
        ///  Binary DeSerializes an object
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The deserialized object</returns>
        public static T BinaryDeSerialize<T>(this byte[] serializedObject)
        {
            Object obj = null;
            if (_formatter == null)
                _formatter = new BinaryFormatter(); 
            
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(serializedObject, 0, serializedObject.Length);
                ms.Seek(0, 0);
                obj = _formatter.Deserialize(ms);
                ms.Close();
            }

            return (T)obj;
        }
        /// <summary>
        /// Serialize the object to a file
        /// </summary>
        /// <param name="obj">Object to be serialized.Ensure that 
        /// is Serializable !</param>
        /// <param name="filePath">File( with the entire file path) 
        // where the object will be serialized to</param>
        public static void FileSerialize(this Object obj, string filePath)
        {
            FileStream fileStream = null;
            if (_formatter == null)
                _formatter = new BinaryFormatter();
            try
            {
                fileStream = new FileStream(filePath, FileMode.Create);
                _formatter.Serialize(fileStream, obj);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        /// <summary>
        /// Deserializes a binary formatted object.
        /// </summary>
        /// <param name="filePath">Full path of the file</param>
        /// <returns>The deserialized object</returns>
        public static T FileDeSerialize<T>(this string filePath)
        {
            FileStream fileStream = null;
            Object obj;
            if (_formatter == null)
                _formatter = new BinaryFormatter(); 
            
            try
            {
                if (File.Exists(filePath) == false)
                {
                    throw new FileNotFoundException("The file was not found.",
                       filePath);
                }
                fileStream = new FileStream(filePath, FileMode.Open);
                obj = _formatter.Deserialize(fileStream);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }

            return (T)obj;
        }

        /// <summary>
        /// Serializes the passed object using SOAP serialization
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="encodingType">The encoding type to use</param>
        /// <returns>A string representing the serialized object.</returns>
        /// <remarks>encodingType is abstract: pass in a 
        /// subtype of Encoding, for example instantiate: 
        /// System.Text.UTF8Encoding</remarks>
        public static string SoapMemoryStreamSerialization(this object obj,
          Encoding encodingType)
        {
            string xmlResult;
            if (_soapFormater == null)
                _soapFormater = new SoapFormatter();

            using (Stream stream = new MemoryStream())
            {
                try
                {
                    _soapFormater.Serialize(stream, obj);
                }
                catch
                {
                    throw;
                }

                stream.Position = 0;
                byte[] b = new byte[stream.Length];
                stream.Read(b, 0, (int)stream.Length);

                xmlResult = encodingType.GetString(b, 0, b.Length);
            }

            return xmlResult;
        }

        /// <summary>
        /// Deserailizes a SOAP serialized object
        /// </summary>
        /// <param name="input">The XML string to deserialize.</param>
        /// <param name="encodingType">The encoding type to use</param>
        /// <returns>The deserialized object.</returns>
        /// <remarks>encodingType is abstract: pass in a 
        /// subtype of Encoding, for 
        /// example instantiate: System.Text.UTF8Encoding</remarks>
        public static T SoapDeserailization<T>(this string input,
                                                System.Text.Encoding encodingType)
        {
            Object obj = null;
            if (_soapFormater == null)
                _soapFormater = new SoapFormatter();

            using (StringReader sr = new StringReader(input))
            {
                byte[] b;
                b = encodingType.GetBytes(input);

                Stream stream = new MemoryStream(b);

                try
                {
                    obj = (object)_soapFormater.Deserialize(stream);
                }
                catch
                {
                    throw;
                }
            }

            return (T)obj;
        }
    }
}
