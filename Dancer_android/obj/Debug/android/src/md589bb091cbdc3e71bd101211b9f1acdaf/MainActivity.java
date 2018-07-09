package md589bb091cbdc3e71bd101211b9f1acdaf;


public class MainActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer,
		android.content.ServiceConnection
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onKeyDown:(ILandroid/view/KeyEvent;)Z:GetOnKeyDown_ILandroid_view_KeyEvent_Handler\n" +
			"n_onServiceConnected:(Landroid/content/ComponentName;Landroid/os/IBinder;)V:GetOnServiceConnected_Landroid_content_ComponentName_Landroid_os_IBinder_Handler:Android.Content.IServiceConnectionInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onServiceDisconnected:(Landroid/content/ComponentName;)V:GetOnServiceDisconnected_Landroid_content_ComponentName_Handler:Android.Content.IServiceConnectionInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("Dancer_android.MainActivity, Dancer_android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MainActivity.class, __md_methods);
	}


	public MainActivity ()
	{
		super ();
		if (getClass () == MainActivity.class)
			mono.android.TypeManager.Activate ("Dancer_android.MainActivity, Dancer_android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public boolean onKeyDown (int p0, android.view.KeyEvent p1)
	{
		return n_onKeyDown (p0, p1);
	}

	private native boolean n_onKeyDown (int p0, android.view.KeyEvent p1);


	public void onServiceConnected (android.content.ComponentName p0, android.os.IBinder p1)
	{
		n_onServiceConnected (p0, p1);
	}

	private native void n_onServiceConnected (android.content.ComponentName p0, android.os.IBinder p1);


	public void onServiceDisconnected (android.content.ComponentName p0)
	{
		n_onServiceDisconnected (p0);
	}

	private native void n_onServiceDisconnected (android.content.ComponentName p0);

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
