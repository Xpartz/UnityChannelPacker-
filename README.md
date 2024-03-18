# Description

This repository contains a Unity script for texture processing tasks, facilitating both texture merging and channel splitting functionalities. The "Texture Packer" script offers a streamlined solution for combining multiple textures into a single RGBA texture, with each source texture occupying a specific channel. Conversely, it also provides the ability to split a merged texture into individual grayscale textures, corresponding to the RGBA channels. This tool enhances texture management workflows within Unity projects, empowering developers to efficiently handle texture operations for various graphical and gaming applications.

Features:

- Merge multiple textures into a single RGBA texture.
- Split a merged texture into individual grayscale textures for each RGBA channel.
- Simplify texture management tasks within Unity projects.

![Plugin window](https://github.com/Xpartz/UnityChannelPacker-/blob/main/Screenshot_1.png)



# Installation

Add the script to any of the Editor folders. After that, go to the Windows menu and find Texture Packer.
![Plugin](https://github.com/Xpartz/UnityChannelPacker-/blob/main/Screenshot_2.png)

#Details

When merging channels, if the texture slot is empty, the channel will be filled with 0.

When splitting channels, if the texture channel is empty, the plugin will not create a texture for that channel.
