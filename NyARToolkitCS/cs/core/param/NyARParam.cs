/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
 * The NyARToolkitCS is C# edition ARToolKit class library.
 * Copyright (C)2008-2009 Ryo Iizuka
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp> or <nyatla(at)nyatla.jp>
 * 
 */
using System.IO;
using System;
using System.Collections.Generic;
namespace jp.nyatla.nyartoolkit.cs.core
{

    /**
     * typedef struct { int xsize, ysize; double mat[3][4]; double dist_factor[4]; } ARParam;
     * NyARの動作パラメータを格納するクラス
     *
     */
    public class NyARParam
    {
        protected NyARIntSize _screen_size = new NyARIntSize();
        private int SIZE_OF_PARAM_SET = 4 + 4 + (3 * 4 * 8) + (4 * 8);
        private NyARCameraDistortionFactor _dist = new NyARCameraDistortionFactor();
        private NyARPerspectiveProjectionMatrix _projection_matrix = new NyARPerspectiveProjectionMatrix();

        public NyARIntSize getScreenSize()
        {
            return this._screen_size;
        }

        public NyARPerspectiveProjectionMatrix getPerspectiveProjectionMatrix()
        {
            return this._projection_matrix;
        }
        public NyARCameraDistortionFactor getDistortionFactor()
        {
            return this._dist;
        }

        /**
         * ARToolKit標準ファイルから1個目の設定をロードする。
         * 
         * @param i_filename
         * @throws NyARException
         */
        public void loadARParamFromFile(string i_filename)
        {
            try
            {
                loadARParam(new StreamReader(i_filename).BaseStream);
            }
            catch (Exception e)
            {
                throw new NyARException(e);
            }
        }

        /**
         * int arParamChangeSize( ARParam *source, int xsize, int ysize, ARParam *newparam );
         * 関数の代替関数 サイズプロパティをi_xsize,i_ysizeに変更します。
         * @param i_xsize
         * @param i_ysize
         * @param newparam
         * @return
         * 
         */
        public void changeScreenSize(int i_xsize, int i_ysize)
        {
            double scale = (double)i_xsize / (double)(this._screen_size.w);// scale = (double)xsize / (double)(source->xsize);
            //スケールを変更
            this._dist.changeScale(scale);
            this._projection_matrix.changeScale(scale);
            //for (int i = 0; i < 4; i++) {
            //	array34[0 * 4 + i] = array34[0 * 4 + i] * scale;// newparam->mat[0][i]=source->mat[0][i]* scale;
            //	array34[1 * 4 + i] = array34[1 * 4 + i] * scale;// newparam->mat[1][i]=source->mat[1][i]* scale;
            //	array34[2 * 4 + i] = array34[2 * 4 + i];// newparam->mat[2][i] = source->mat[2][i];
            //}


            this._screen_size.w = i_xsize;// newparam->xsize = xsize;
            this._screen_size.h = i_ysize;// newparam->ysize = ysize;
            return;
        }

        public void loadARParam(Stream i_stream)
        {
            loadARParam(new BinaryReader(i_stream));
        }
        /**
         * int arParamLoad( const char *filename, int num, ARParam *param, ...);
         * i_streamの入力ストリームからi_num個の設定を読み込み、パラメタを配列にして返します。
         * 
         * @param i_stream
         * @throws Exception
         */
        public void loadARParam(BinaryReader i_reader)
        {
            try
            {
                byte[] buf = new byte[SIZE_OF_PARAM_SET];
                double[] tmp = new double[12];

                // バッファを加工
                this._screen_size.w = endianConv(i_reader.ReadInt32());
                this._screen_size.h = endianConv(i_reader.ReadInt32());
                //double値を12個読み込む
                for (int i = 0; i < 12; i++)
                {
                    tmp[i] = endianConv(i_reader.ReadDouble());
                }
                //Projectionオブジェクトにセット
                this._projection_matrix.setValue(tmp);
                //double値を4個読み込む
                for (int i = 0; i < 4; i++)
                {
                    tmp[i] = endianConv(i_reader.ReadDouble());
                }
                //Factorオブジェクトにセット
                this._dist.setValue(tmp);
            }
            catch (Exception e)
            {
                throw new NyARException(e);
            }
            return;
        }

        public void saveARParam(StreamWriter i_stream)
        {
            NyARException.trap("未チェックの関数");
/*            byte[] buf = new byte[SIZE_OF_PARAM_SET];
            // バッファをラップ
            ByteBuffer bb = ByteBuffer.wrap(buf);
            bb.order(ByteOrder.BIG_ENDIAN);

            // 書き込み
            bb.putInt(this._screen_size.w);
            bb.putInt(this._screen_size.h);
            double[] tmp = new double[12];
            //Projectionを読み出し
            this._projection_matrix.getValue(tmp);
            //double値を12個書き込む
            for (int i = 0; i < 12; i++)
            {
                tmp[i] = bb.getDouble();
            }
            //Factorを読み出し
            this._dist.getValue(tmp);
            //double値を4個書き込む
            for (int i = 0; i < 4; i++)
            {
                tmp[i] = bb.getDouble();
            }
            i_stream.write(buf);
            return;
 */
        }
        private static double endianConv(double i_val)
        {
            if (!BitConverter.IsLittleEndian)
            {
                return i_val;
            }
            byte[] ba = BitConverter.GetBytes(i_val);
            Array.Reverse(ba);
            return BitConverter.ToDouble(ba, 0);
        }
        private static int endianConv(int i_val)
        {
            if (!BitConverter.IsLittleEndian)
            {
                return i_val;
            }
            byte[] ba = BitConverter.GetBytes(i_val);
            Array.Reverse(ba);
            return BitConverter.ToInt32(ba, 0);
        }
    }
}