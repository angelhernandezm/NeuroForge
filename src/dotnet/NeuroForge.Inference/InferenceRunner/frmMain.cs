using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace InferenceRunner {
    /// <summary>
    /// Class frmMain.
    /// Implements the <see cref="System.Windows.Forms.Form" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class frmMain : Form {
        /// <summary>
        /// Enum FileType
        /// </summary>
        enum FileType {
            Image, ONNX
        }

        /// <summary>
        /// The class names
        /// </summary>
        private readonly string[] _classNames = ["airplane", "automobile", "bird", "cat", "deer", "dog", "frog", "horse", "ship", "truck"];

        /// <summary>
        /// Initializes a new instance of the <see cref="frmMain"/> class.
        /// </summary>
        public frmMain() {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the button1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void button1_Click(object sender, EventArgs e) => SelectFile(FileType.ONNX);

        /// <summary>
        /// Handles the Click event of the button2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void button2_Click(object sender, EventArgs e) => SelectFile(FileType.Image);

        /// <summary>
        /// Selects the file.
        /// </summary>
        private void SelectFile(FileType fileType) {
            using (var openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                openFileDialog.Filter = fileType == FileType.ONNX ? "ONNX files (*.onnx)|*.onnx" : "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    //Get the path of specified file
                    var filePath = openFileDialog.FileName;
                    if (fileType == FileType.ONNX) {
                        textBox1.Text = filePath;
                    } else {
                        textBox2.Text = filePath;
                    }
                }
            }
        }

        /// <summary>
        /// Resizes the to32.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Bitmap.</returns>
        private Bitmap ResizeTo32(Image image) {
            var retval = new Bitmap(32, 32);
            var destRect = new Rectangle(0, 0, 32, 32);
            retval.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(retval)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height,
                                       GraphicsUnit.Pixel, wrapMode);
                }
            }

            return retval;
        }

        /// <summary>
        /// Preprocesses the image.
        /// </summary>
        /// <returns>float[].</returns>
        private float[] PreprocessImage() {
            var retval = new float[1 * 3 * 32 * 32];

            using (var img = Image.FromFile(textBox2.Text)) {
                var idx = 0;
                var resized = ResizeTo32(img);
                for (var y = 0; y < 32; y++) {
                    for (var x = 0; x < 32; x++) {
                        var pixel = resized.GetPixel(x, y);
                        // Normalize to [0,1]
                        retval[idx++] = pixel.R / 255f;
                        retval[idx++] = pixel.G / 255f;
                        retval[idx++] = pixel.B / 255f;
                    }
                }

                return retval;
            }
        }

        /// <summary>
        /// Handles the Click event of the button3 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void button3_Click(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text)) {
                var session = new InferenceSession(textBox1.Text);
                var tensor = PreprocessImage();
                var inputTensor = new DenseTensor<float>(tensor, new[] { 1, 32, 32, 3 });
                var inputName = session.InputMetadata.Keys.First();
                var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(inputName, inputTensor) };
                using var results = session.Run(inputs);
                var output = results.First().AsEnumerable<float>().ToArray();

                // ArgMax (finds the index of the maximum value in  output)
                //   https://en.wikipedia.org/wiki/Arg_max
                //   https://numpy.org/doc/stable/reference/generated/numpy.argmax.html
                var maxIdx = output.Select((value, index) => (value, index)).Aggregate((a, b) => b.value > a.value ? b : a).index;
                
                MessageBox.Show($"Prediction: {_classNames[maxIdx]} (score: {output[maxIdx]:F4})", "Inference Result");
            }

        }
    }
}

