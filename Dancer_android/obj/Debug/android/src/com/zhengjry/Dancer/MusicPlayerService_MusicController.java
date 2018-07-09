package com.zhengjry.Dancer;


public class MusicPlayerService_MusicController
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Dancer_android.MusicPlayerService+MusicController, Dancer_android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MusicPlayerService_MusicController.class, __md_methods);
	}


	public MusicPlayerService_MusicController ()
	{
		super ();
		if (getClass () == MusicPlayerService_MusicController.class)
			mono.android.TypeManager.Activate ("Dancer_android.MusicPlayerService+MusicController, Dancer_android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
