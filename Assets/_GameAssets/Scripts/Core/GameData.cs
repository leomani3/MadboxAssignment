using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

[CreateAssetMenu(menuName = "Config/GameData")]
public class GameData : ScriptableObject
{
	[ES3NonSerializable] public Action<CurrencyAsset, double> onCurrencyChanged;
	[ES3NonSerializable] public Action<CurrencyAsset, double> onCurrencyAdded;
	[ES3NonSerializable] public Action<CurrencyAsset> onNotEnoughCurrency;
	[ES3NonSerializable] public Action OnResetData;

	private static GameData _instance;
	public static GameData Instance => _instance ?? Load();

	public void Init()
	{
	}

	[Button]
	public void Save()
	{
		ES3.Save("GameData", this);
		isDirty = false;
	}

	private static GameData Load()
	{
		_instance = Resources.Load<GameData>("GameData");
		if (ES3.KeyExists("GameData"))
			ES3.LoadInto("GameData", _instance);
		else
			ResetGameData();

		return _instance;
	}


	// ---------------------------------------------------------
	[ES3NonSerializable] public bool isDirty = false;

	#region General
	
	public static void ResetGameData()
	{

		Instance.Save();
		Instance.OnResetData?.Invoke();
	}

	public void ResetRun()
	{
		Save();
	}

	#endregion
	
	#region Currencies
	[Header("Currencies")]
	public SerializableDictionary<Currency, double> currencies = new SerializableDictionary<Currency, double>();

	[Button]
	public void AddCurrency(CurrencyAsset _currencyAsset, double amount, bool forceSave = false)
	{
		Currency currency = _currencyAsset.Currency;
		if (_currencyAsset == null) return;

		if (currencies == null)
			currencies = new SerializableDictionary<Currency, double>();
		
		if (currencies.ContainsKey(currency))
		{
			currencies[currency] += amount;
		}
		else
		{
			currencies[currency] = amount;
		}
		
		onCurrencyChanged?.Invoke(_currencyAsset, currencies[currency]);
		onCurrencyAdded?.Invoke(_currencyAsset, amount);

		isDirty = true;
	}

	public bool SpendCurrency(CurrencyAsset _currencyAsset, double amount)
	{
		Currency currency = _currencyAsset.Currency;
		if (_currencyAsset == null || currencies == null) return false;

		if (currencies.ContainsKey(currency))
		{
			double current = currencies[currency];
			double newValue = current > amount ? current - amount : 0UL;
			currencies[currency] = newValue;

			onCurrencyChanged?.Invoke(_currencyAsset, currencies[currency]);
			Save();
			return true;
		}
		return false;
	}

	public bool HasEnoughCurrency(CurrencyAsset _currencyAsset, double _amount, bool _feedBack = false)
	{
		Currency currency = _currencyAsset.Currency;

		if (_amount == 0)
			return true;

		if (_currencyAsset == null || currencies == null || !currencies.ContainsKey(currency) || currencies[currency] < _amount)
		{
			if (_feedBack)
				onNotEnoughCurrency?.Invoke(_currencyAsset);
			return false;
		}

		return true;
	}
	

	public void DepleteCurrency(CurrencyAsset _currencyAsset)
	{
		Currency currency = _currencyAsset.Currency;
		if (currencies.ContainsKey(currency))
		{
			currencies[currency] = 0;
			onCurrencyChanged?.Invoke(_currencyAsset, 0);
			Save();
		}
	}
	
	#endregion
}