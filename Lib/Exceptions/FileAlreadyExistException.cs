using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSEditor.Exceptions {
	public class FileAlreadyExistException : ApplicationException {
        private string[] Files { get; set; }
        public FileAlreadyExistException() : base("A file with the same name under that location already exists.") { }
		public FileAlreadyExistException(string message, string[] files) : base(message) {
			Files = files;
		}
    }
}
