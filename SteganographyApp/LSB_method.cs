using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace LSB_method
{
    class LSB_method
    {
        private Bitmap image = null;
        private Tuple<int, int> image_shape = null;
        private long image_lenght;
        private readonly string stop_bytes;
        private readonly bool isHorizontal;
        private readonly string formats = "Image files|*.bmp;*.png;*.jpg";

        public LSB_method(string stop, bool isHorizontal)
        {
            this.stop_bytes = stop;
            this.isHorizontal = isHorizontal;
        }

        public LSB_method()
        {
            stop_bytes = "<E>";
            isHorizontal = true;
        }

        private byte[] Get_stop_bytes()
        {
            return String_to_bytes(stop_bytes);
        }

        public void Load_from_path(string path)
        {
            image = new Bitmap(path);
            image_shape = new Tuple<int,int>(image.Width, image.Height);
            image_lenght = image.Width * image.Height;
        }

        public void Load(Bitmap image)
        {
            this.image = image;
            image_shape = new Tuple<int, int>(image.Width, image.Height);
            image_lenght = image.Width * image.Height;
        }

        public void Save_in_path(Bitmap image, string path)
        {
            string[] path_array = path.Split('.');

            string format = path_array[path_array.Length - 1].ToLower();

            using (var ms = new MemoryStream())
            {
                if (format == "bmp")
                 {
                    image.Save(path, ImageFormat.Bmp);
                 }
                 else if (format == "png")
                 {
                     image.Save(path, ImageFormat.Png);
                 }
                 else if(format == "jpg")
                 {
                     image.Save(path, ImageFormat.Jpeg);
                 }
            }
        }

        public string Get_formats()
        {
            return formats;
        }

        public Bitmap Get_image()
        {
            return image;
        }

        private byte[] String_to_bytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        private string Bytes_to_string(byte[] bytes)
        {
            
            return Encoding.ASCII.GetString(bytes);
        }

        private bool[] Bytes_to_bool_array(byte[] bytes)
        {
            bool[] result = new bool[bytes.Length * 8];

            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                bool[] local = new bool[8];
                for (int k = 0; k < 8; k++)
                {
                    local[k] = (b & (1 << k)) == 0 ? false : true;
                }
                Array.Reverse(local);
                Array.Copy(local, 0, result, i * 8, 8);

            }
            
            return result;

        }

        private bool[] Fill_ones_to_24(bool[] array)
        {
            bool[] new_array = new bool[24];

            for (int i = 0; i < array.Length; i++)
            {
                new_array[i] = array[i];
            }
            for (int i = array.Length; i < 24; i++)
            {
                new_array[i] = true;
            }

            return new_array;
        }

        private byte Change_LSB(byte color, bool LSB)
        {
            if (LSB) return (byte)(color | 1);
            else return (byte)(color & 254);
        }

        private void Write_bool_to_pixel(bool r, bool g, bool b, int offset)
        {
            Tuple<int, int> position;
            if (isHorizontal)
            {
                position = new Tuple<int, int>(offset % image_shape.Item1, offset / image_shape.Item1);
            }
            else
            {
                position = new Tuple<int, int>(offset / image_shape.Item2, offset % image_shape.Item2);
            }

            Color color = image.GetPixel(position.Item1, position.Item2);
            Color new_color = Color.FromArgb(color.A,
                                            Change_LSB(color.R, r),
                                            Change_LSB(color.G, g),
                                            Change_LSB(color.B, b));
            image.SetPixel(position.Item1, position.Item2, new_color);
        }

        private void Write_bool_array(bool[] array, int offset)
        {
            for (int i = 0; i < array.Length; i += 3)
            {
                Write_bool_to_pixel(array[i], array[i + 1], array[i + 2], offset+i/3);
            }
        }

        private void Preprocessing_before_writting(byte[] bytes, int offset)
        {
            bool[] array = Bytes_to_bool_array(bytes);
            array = Fill_ones_to_24(array);
            Write_bool_array(array, offset);
        }


        public void Encrypt_text(string text_string)
        {
            byte[] bytes_old = String_to_bytes(text_string);
            
            byte[] end_point = Get_stop_bytes();
            byte[] bytes = new byte[bytes_old.Length + end_point.Length];
            Array.Copy(bytes_old, 0, bytes, 0, bytes_old.Length);
            Array.Copy(end_point, 0, bytes, bytes_old.Length, end_point.Length);

            int len = bytes.Length / 3;
            int i = 0;
            for(i=0; i<len; i++)
            {
                byte[] local = new byte[3];
                Array.Copy(bytes, 3 * i, local, 0, 3);
                Preprocessing_before_writting(local, i*8);
            }
            if(bytes.Length-(len*3) == 2)
            {
                byte[] local = new byte[2];
                Array.Copy(bytes, 3*i, local, 0, 2);
                Preprocessing_before_writting(local, i*8);
            }
            else if(bytes.Length-(len*3) == 1)
            {
                byte[] local = new byte[1];
                Array.Copy(bytes, 3*i, local, 0, 1);
                Preprocessing_before_writting(local, i*8);
            }
        }

        private string Bool_to_string(bool b)
        {
            if (b) return "1";
            else return "0";
        }

        private bool[] Get_pixel(int offset)
        {
            bool[] result = new bool[3];
            Tuple<int, int> position;
            if (isHorizontal)
            {
                position = new Tuple<int, int>(offset % image_shape.Item1, offset / image_shape.Item1);
            }
            else
            {
                position = new Tuple<int, int>(offset / image_shape.Item2, offset % image_shape.Item2);
            }
            Color color = image.GetPixel(position.Item1, position.Item2);
            result[0] = Get_LSB_from_byte(color.R);
            result[1] = Get_LSB_from_byte(color.G);
            result[2] = Get_LSB_from_byte(color.B);

            return result;
        }

        private bool Get_LSB_from_byte(byte b)
        {
            return (b & (1 << 0)) != 0;
        }

        public byte[] Get_bytes(int offset)
        {
            bool[] bool_array = new bool[24];
            for(int i=0; i<8; i++)
            {
                Array.Copy(Get_pixel(offset+i), 0, bool_array, i * 3, 3);
            }
            return Bool_array_to_bytes(bool_array);
        }

        private byte[] Bool_array_to_bytes(bool[] bool_array)
        {
            byte[] result = new byte[bool_array.Length / 8];

            for(int i=0; i< bool_array.Length / 8; i++)
            {
                bool[] bools = new bool[8];
                Array.Copy(bool_array, i * 8, bools, 0, 8);
                int index = 8 - bools.Length;
                byte local = 0;
                foreach (bool b in bools)
                {
                    if (b)
                        local |= (byte)(1 << (7 - index));
                    index++;
                }
                result[i] = local;
            }
            return result;
        }

        private bool Is_end(byte first, byte second, byte third)
        {
            byte[] stop = Get_stop_bytes();
            if (first != stop[0]) return false;
            else if (second != stop[1]) return false;
            else if (third != stop[2]) return false;
            else return true;

        }



        public string Decrypt_text()
        {
            byte[] final = null;
            byte[] result = new byte[0];
            byte[] stop = Get_stop_bytes();
            byte[] buffer;

            for (int i = 0; i < image_lenght - 8; i += 8)
            {
                buffer = Get_bytes(i);

                byte[] local = new byte[result.Length + buffer.Length];
                Array.Copy(result, 0, local, 0, result.Length);
                Array.Copy(buffer, 0, local, result.Length, buffer.Length);


                if (result.Length >= 3)
                {
                    if (Is_end(buffer[0], buffer[1], buffer[2]))
                    {
                        final = result;
                        break;
                    }
                    else if (Is_end(result[result.Length - 1], buffer[0], buffer[1]))
                    {
                        byte[] pre_result = new byte[result.Length - 1];
                        Array.Copy(result, 0, pre_result, 0, result.Length - 1);
                        final = pre_result;
                        break;
                    }
                    else if (Is_end(result[result.Length - 2], result[result.Length - 1], buffer[0]))
                    {
                        byte[] pre_result = new byte[result.Length - 2];
                        Array.Copy(result, 0, pre_result, 0, result.Length - 2);
                        final= pre_result;
                        break;
                    }
                }

                result = local;
            }

            if (final != null)
            {
                return Bytes_to_string(final);
            }
            else
            {
                return null;
            }
            
        }

        public Tuple<byte[], string> Decrypt_bytes()
        {
            Tuple<byte[], string> final = null;
            byte[] result = new byte[0];
            byte[] stop = Get_stop_bytes();
            byte[] buffer;

            for (int i = 0; i < image_lenght - 8; i += 8)
            {
                buffer = Get_bytes(i);

                byte[] local = new byte[result.Length + buffer.Length];
                Array.Copy(result, 0, local, 0, result.Length);
                Array.Copy(buffer, 0, local, result.Length, buffer.Length);


                if (result.Length >= 3)
                {
                    if (Is_end(buffer[0], buffer[1], buffer[2]))
                    {
                        if(final == null)
                        {
                            final = new Tuple<byte[], string>(result, "");
                        }
                        else
                        {
                            byte[] extension = new byte[result.Length - final.Item1.Length - 3];
                            Array.Copy(result, final.Item1.Length + 3, extension, 0, extension.Length);

                            final = new Tuple<byte[], string>(final.Item1, Bytes_to_string(extension));
                            break;
                        }
                        
                    }
                    else if (Is_end(result[result.Length - 1], buffer[0], buffer[1]))
                    {
                        if(final == null)
                        {
                            byte[] pre_result = new byte[result.Length - 1];
                            Array.Copy(result, 0, pre_result, 0, result.Length - 1);
                            final = new Tuple<byte[], string>(pre_result, "");

                        }
                        else
                        {
                            byte[] extension = new byte[result.Length - final.Item1.Length - 4];
                            Array.Copy(result, final.Item1.Length + 3, extension, 0, extension.Length);

                            final = new Tuple<byte[], string>(final.Item1, Bytes_to_string(extension));
                            break;
                        }
                    }
                    else if (Is_end(result[result.Length - 2], result[result.Length - 1], buffer[0]))
                    {
                        if(final == null)
                        {
                            byte[] pre_result = new byte[result.Length - 2];
                            Array.Copy(result, 0, pre_result, 0, result.Length - 2);
                            final = new Tuple<byte[], string>(pre_result, "");
                        }
                        else
                        {
                            byte[] extension = new byte[result.Length - final.Item1.Length -5];
                            Array.Copy(result, final.Item1.Length + 3, extension, 0, extension.Length);

                            final = new Tuple<byte[], string>(final.Item1, Bytes_to_string(extension));
                            break;
                        }
                    }
                }

                result = local;
            }

            return final;
        }

        public void Encrypt_bytes(byte[] bytes_old, string ext)
        {
            byte[] end_point = Get_stop_bytes();
            byte[] file_ex = String_to_bytes(ext);
            byte[] bytes = new byte[bytes_old.Length + 2*end_point.Length + file_ex.Length];
            Array.Copy(bytes_old, 0, bytes, 0, bytes_old.Length);
            Array.Copy(end_point, 0, bytes, bytes_old.Length, end_point.Length);

            Array.Copy(file_ex, 0, bytes, bytes_old.Length+end_point.Length, file_ex.Length);
            Array.Copy(end_point, 0, bytes, bytes_old.Length + end_point.Length + file_ex.Length, end_point.Length);

            int len = bytes.Length / 3;
            int i = 0;
            for (i = 0; i < len; i++)
            {
                byte[] local = new byte[3];
                Array.Copy(bytes, 3 * i, local, 0, 3);
                Preprocessing_before_writting(local, i * 8);
            }
            if (bytes.Length - (len * 3) == 2)
            {
                byte[] local = new byte[2];
                Array.Copy(bytes, 3 * i, local, 0, 2);
                Preprocessing_before_writting(local, i * 8);
            }
            else if (bytes.Length - (len * 3) == 1)
            {
                byte[] local = new byte[1];
                Array.Copy(bytes, 3 * i, local, 0, 1);
                Preprocessing_before_writting(local, i * 8);
            }
        }

        public Tuple<int, int> Get_shape_image()
        {
            return image_shape;
        }

        public long Get_lenght_image()
        {
            return image_lenght;
        }
    }
}
