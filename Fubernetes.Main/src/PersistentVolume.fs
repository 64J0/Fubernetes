namespace Fubernetes.Resources

open System
open System.IO

module PersistentVolume =
    type PersistentVolumeConstructor = { Name: string }
