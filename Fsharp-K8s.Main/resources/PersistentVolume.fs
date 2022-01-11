namespace FsharpK8s.Resources

open System
open System.IO

module PersistentVolume =
    type PersistentVolumeConstructor =
        { Name: string }