BlackNet - .NET (C#) wrapper for the BeagleBone Black
========

Provides .NET access to the BBB's IOs through the BBB's Linux file drivers.  Configures the device tree (optionally) as necessary.

The following IOs are current complete:

* PWM
* GPIO

If you're using the other IOs, please contribute to this library.

The following projects are included:

* BlackNet.csproj - This contains the IO device wrappers
* BlackNet.Test - Unit tests - should run on any hardware (PC)
* BlackNet.InteractiveTests - To be run on the BBB to allows command-line experimentation with the ports
* ConsoleUi - Simple menuing system used by the InteractiveTests project

Setting up Mono on the BeagleBone Black
---------------------------------------

Unfortunately, as of this writing (Dec 22, 2014), there isn't a mono package for Weezy on ARMhf, otherwise installing mono would be as easy as this command (on Debian):

    # DOESN'T WORK - there is not a package available yet.  But see below...
    sudo apt-get install mono-complete
  
You could pull down the source and let the BBB churn on compiling that for about a day, but fortunately, "pingfu" 
has kindly provided a [.deb package for us](http://pingfu.net/programming/troubleshooting/hardware/2014/10/23/mono-debian-package-armhf-beaglebone-black.html).  

Make sure that you setup networking on your BBB first:
* Setup your network interface to share with the USB adapter.
* Setup your BBB to use the USB host as the gateway:

  	    /sbin/route add default gw 192.168.7.1

Update your package list:

    sudo apt-get update
  
Now download [his package](https://s3-eu-west-1.amazonaws.com/westgatecyber/mono-3.8.0-branch-armhf-e451fb2.deb) then install it like this:

    sudo dpkg -i mono*.deb
  
You can download the above file using commands, or download the file on your host computer and SCP it (I use WinSCP) to your BBB.

Compiling And Running
---------------------

There are various options for compiling and running on Mono on the BBB.  
* If you are using HDMI or VNC you can install MonoDevelop on the BeagleBone Black and use it as per normal.  
* What I've been trying to get working is remote debugging from Visual Studio.  The Mono soft debugger appears to be rather mature, as it's used by Unity 
and Xamarin, and I think even when debugging locally by the MonoDevelop environment, but integration with
Visual Studio is the main issue.  

There is a promising looking Visual Studio plugin, [MonoDebugger](https://github.com/giessweinapps/MonoDebugger)
which automatically packages up the build output, copies it to the Linux target, converts the PDBs to MDBs and 
starts the soft debugger, but unfortunately the present state of the VS debugger integration is so limited as
to be nearly useless.  So far I've found it to only work for a single thread and to inspect local variables.  
I wish I had more time to give to help that project along because it would be so useful if it worked well.

Xamarin already has a fairly robust integration to the mono soft debugger in their commercial VS plugin, I'd
definitely fork over the cash if they'd generalize that to work with regular Linux Mono (hint hint  :-)  ).

Please let us know if you find a better debugging option.
