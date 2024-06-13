using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemForensics.Models;

public class Volume
{
    public string Name { get; set; }
    public string Label { get; set; }
    public string Type { get; set; }
    public string Format { get; set; }
    public double TotalSize { get; set; }
    public double SpaceUsed { get; set; }
    public double SpaceAvailable { get; set; }
}
